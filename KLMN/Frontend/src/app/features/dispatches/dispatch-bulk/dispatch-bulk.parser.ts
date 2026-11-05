import type { DispatchDetails, DispatchPerson } from '../models/dispatch.model';

/** Tek istekte sınırlı veri işleyerek SQL, bellek ve tarayıcı kaynaklarını koruruz. */
export const MAX_BULK_DISPATCH_ROWS = 100;
export const MAX_BULK_DISPATCH_FILE_BYTES = 1024 * 1024;

export interface BulkDispatchItem {
  person: DispatchPerson;
  dispatch: DispatchDetails;
}
export interface BulkParseProblem {
  rowNumber: number;
  message: string;
}
export interface BulkParseResult {
  items: BulkDispatchItem[];
  problems: BulkParseProblem[];
}

type RowObject = Record<string, unknown>;
const GUID = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

function object(value: unknown): RowObject | null {
  return value !== null && typeof value === 'object' && !Array.isArray(value)
    ? value as RowObject : null;
}
function pick(row: RowObject, ...keys: string[]): unknown {
  for (const key of keys) if (Object.prototype.hasOwnProperty.call(row, key)) return row[key];
  return null;
}
function text(value: unknown): string | null {
  if (typeof value === 'string') return value.trim() || null;
  if (typeof value === 'number' && Number.isFinite(value)) return String(value);
  return null;
}
function number(value: unknown): number | null {
  if (value === '' || value === null || value === undefined) return null;
  const parsed = Number(value);
  return Number.isSafeInteger(parsed) ? parsed : null;
}
function date(value: unknown): string | null {
  const s = text(value);
  return s && /^\d{4}-\d{2}-\d{2}(?:T|$)/.test(s) ? s.slice(0, 10) : s;
}
function guid(value: string | null): boolean {
  return !!value && GUID.test(value) && !/^0{8}-0{4}-0{4}-0{4}-0{12}$/i.test(value);
}

/** Eski .xml dosyası gerçekte JSON içeriyordu; gerçek XML ayrıştırdığımızı iddia etmiyoruz. */
function getArray(source: string): unknown[] {
  const raw = source.replace(/^\uFEFF/, '').trim();
  if (!raw) throw new Error('Seçilen dosya boş.');
  if (raw.startsWith('<')) {
    throw new Error('Dosya gerçek XML içeriyor. Eski ekranda .xml uzantısı kullanılsa da içerik JSON idi. XML şeması eşleştirilmeden aktarılamaz.');
  }

  let data: unknown;
  try {
    data = JSON.parse(raw);
  } catch {
    try {
      // Eski dışa aktarımdaki {"...":...},{"...":...} dizisini güvenle ayrıştırır.
      data = JSON.parse('[' + raw.replace(/,\s*$/, '') + ']');
    } catch {
      try {
        data = raw.split(/\r?\n/).map(line => line.trim()).filter(Boolean).map(line => JSON.parse(line));
      } catch {
        throw new Error('Dosya JSON biçiminde değil. JSON dizi, sevkYeniGiris dizisi veya satır bazlı JSON kullanınız.');
      }
    }
  }

  if (Array.isArray(data)) return data;
  const wrapper = object(data);
  if (wrapper) {
    const list = pick(wrapper,'items','Items','sevkYeniGiris','SevkYeniGiris');
    if (Array.isArray(list)) return list;
    if (object(pick(wrapper,'person','Person')) && object(pick(wrapper,'dispatch','Dispatch'))) return [wrapper];
  }
  throw new Error('Dosyada sevk listesi bulunamadı. Örnek biçim: {"items":[{"person":{...},"dispatch":{...}}]}.');
}

/** Hem yeni {person,dispatch} hem bilinen eski düz alan adları desteklenir. */
function parseRow(value: unknown): BulkDispatchItem {
  const row = object(value);
  if (!row) throw new Error('Satır bir JSON nesnesi olmalıdır.');
  const sourcePerson = object(pick(row,'person','Person')) ?? row;
  const sourceDispatch = object(pick(row,'dispatch','Dispatch')) ?? row;

  const person: DispatchPerson = {
    tcKimlikNo: text(pick(sourcePerson,'tcKimlikNo','TcKimlikNo')),
    yabanciKimlikNo: text(pick(sourcePerson,'yabanciKimlikNo','YabanciKimlikNo')),
    name: text(pick(sourcePerson,'name','Name','adi','Adi')) ?? '',
    surname: text(pick(sourcePerson,'surname','Surname','soyadi','Soyadi')) ?? '',
    motherName: text(pick(sourcePerson,'motherName','MotherName','anneAdi','AnneAdi')),
    fatherName: text(pick(sourcePerson,'fatherName','FatherName','babaAdi','BabaAdi')),
    nationalityId: text(pick(sourcePerson,'nationalityId','NationalityId')),
    townOfBirthId: text(pick(sourcePerson,'townOfBirthId','TownOfBirthId')),
    birthDate: date(pick(sourcePerson,'birthDate','BirthDate','dogumTarihi','DogumTarihi')) ?? '',
    gender: number(pick(sourcePerson,'gender','Gender','cinsiyet')),
    phoneNumber: text(pick(sourcePerson,'phoneNumber','PhoneNumber','telefonNo')),
    address: text(pick(sourcePerson,'address','Address','adres')),
    birthPlaceText: text(pick(sourcePerson,'birthPlaceText','BirthPlaceText')),
    birthPlaceCode: text(pick(sourcePerson,'birthPlaceCode','BirthPlaceCode'))
  };
  const scope = number(pick(sourceDispatch,'dispatchScope','DispatchScope','sevkKapsami','SevkKapsami')) ?? 0;
  let senderUnit = text(pick(sourceDispatch,'senderMilitaryUnitDefId','SenderMilitaryUnitDefId'));
  let senderOffice = text(pick(sourceDispatch,'senderMilitaryOfficeDefId','SenderMilitaryOfficeDefId'));
  const senderChoice = text(pick(sourceDispatch,'senderChairChoice','SenderChairChoice'));
  const typedSender = /^(unit|office):([0-9a-f-]{36})$/i.exec(senderChoice ?? '');
  if (!senderUnit && !senderOffice && typedSender) {
    if (typedSender[1].toLowerCase() === 'unit') senderUnit = typedSender[2];
    else senderOffice = typedSender[2];
  }
  const dispatch: DispatchDetails = {
    nthAdmission: number(pick(sourceDispatch,'nthAdmission','NthAdmission','kacinciIslem')),
    dispatchDocumentDate: date(pick(sourceDispatch,'dispatchDocumentDate','DispatchDocumentDate','sevkEvrakTarihi')),
    dispatchDocumentNo: text(pick(sourceDispatch,'dispatchDocumentNo','DispatchDocumentNo','sevkEvrakSayisi')),
    reasonForExaminationDefId: text(pick(sourceDispatch,'reasonForExaminationDefId','ReasonForExaminationDefId','muayeneSebebi')),
    dispatchScope: scope,
    dispatchScopeCityId: text(pick(sourceDispatch,'dispatchScopeCityId','DispatchScopeCityId','sevkKapsamiSehir')),
    dispatchScopeHospitalId: text(pick(sourceDispatch,'dispatchScopeHospitalId','DispatchScopeHospitalId','sevkKapsamiHastane')),
    hcAdmissionTypeDefId: text(pick(sourceDispatch,'hcAdmissionTypeDefId','HcAdmissionTypeDefId','skBasvuruGrubu')),
    forcesCommandDefId: text(pick(sourceDispatch,'forcesCommandDefId','ForcesCommandDefId','kuvvet')),
    rankDefId: text(pick(sourceDispatch,'rankDefId','RankDefId','rutbe')),
    militaryClassDefId: text(pick(sourceDispatch,'militaryClassDefId','MilitaryClassDefId','sinif')),
    militaryUnitDefId: text(pick(sourceDispatch,'militaryUnitDefId','MilitaryUnitDefId','birlik')),
    militarySchoolDefId: text(pick(sourceDispatch,'militarySchoolDefId','MilitarySchoolDefId','okulu')),
    senderMilitaryUnitDefId: senderUnit,
    senderMilitaryOfficeDefId: senderOffice,
    senderChairOther: text(pick(sourceDispatch,'senderChairOther','SenderChairOther')),
    militaryOfficeDefId: text(pick(sourceDispatch,'militaryOfficeDefId','MilitaryOfficeDefId')),
    flyingMissionDefId: text(pick(sourceDispatch,'flyingMissionDefId','FlyingMissionDefId')),
    planeTypeDefId: text(pick(sourceDispatch,'planeTypeDefId','PlaneTypeDefId')),
    registryNumber: text(pick(sourceDispatch,'registryNumber','RegistryNumber','sicilNo')),
    lastNominationDate: date(pick(sourceDispatch,'lastNominationDate','LastNominationDate','nasbi')),
    militaryUnitDescription: text(pick(sourceDispatch,'militaryUnitDescription','MilitaryUnitDescription','birlikAciklama')),
    purchaseId: number(pick(sourceDispatch,'purchaseId','PurchaseId','alimId')),
    description: text(pick(sourceDispatch,'description','Description'))
  };

  if (!person.name || !person.surname) throw new Error('Ad ve soyad gereklidir.');
  if (!!person.tcKimlikNo === !!person.yabanciKimlikNo)
    throw new Error('T.C. veya yabancı kimlik numarasından yalnız biri girilmelidir.');
  if (person.tcKimlikNo && !/^[1-9][0-9]{10}$/.test(person.tcKimlikNo))
    throw new Error('T.C. kimlik numarası 11 haneli ve sayısal olmalıdır.');
  if (person.yabanciKimlikNo && !/^[0-9]{11,20}$/.test(person.yabanciKimlikNo))
    throw new Error('Yabancı kimlik numarası 11-20 haneli sayısal olmalıdır.');
  if (!/^\d{4}-\d{2}-\d{2}$/.test(person.birthDate))
    throw new Error('Doğum tarihi yyyy-AA-gg biçiminde olmalıdır.');
  if (!guid(dispatch.hcAdmissionTypeDefId)) throw new Error('Başvuru grubu GUID kimliği gereklidir.');
  if (!guid(dispatch.reasonForExaminationDefId)) throw new Error('Muayene sebebi GUID kimliği gereklidir.');
  if (scope !== 2 && scope !== 3) throw new Error('Sevk kapsamı 2 (şehir) veya 3 (sağlık tesisi) olmalıdır.');
  if (scope === 2 && !guid(dispatch.dispatchScopeCityId)) throw new Error('Şehir kapsamı için şehir GUID kimliği gereklidir.');
  if (scope === 3 && !guid(dispatch.dispatchScopeHospitalId)) throw new Error('Sağlık tesisi kapsamı için hastane GUID kimliği gereklidir.');
  if (scope === 2 && dispatch.dispatchScopeHospitalId || scope === 3 && dispatch.dispatchScopeCityId)
    throw new Error('Sevk kapsamıyla ilgisiz şehir/hastane alanı boş bırakılmalıdır.');
  if (!!senderUnit === !!senderOffice || (senderUnit !== null && !guid(senderUnit)) || (senderOffice !== null && !guid(senderOffice)))
    throw new Error('Gönderen makamı senderMilitaryUnitDefId, senderMilitaryOfficeDefId veya unit:/office: GUID olarak belirtiniz.');
  return {person,dispatch};
}

/** Geçersiz satırlar sessizce atılmaz; yükleme tümüyle engellenir. */
export function parseBulkDispatchFile(content: string): BulkParseResult {
  const raw = getArray(content);
  if (raw.length === 0) throw new Error('Dosyada sevk bulunamadı.');
  if (raw.length > MAX_BULK_DISPATCH_ROWS)
    throw new Error('Bir dosyada en fazla ' + MAX_BULK_DISPATCH_ROWS + ' sevk bulunabilir.');
  const items: BulkDispatchItem[] = [];
  const problems: BulkParseProblem[] = [];
  for (let index = 0; index < raw.length; index++) {
    try { items.push(parseRow(raw[index])); }
    catch (e) {
      problems.push({rowNumber:index+1,message:e instanceof Error?e.message:'Satır okunamadı.'});
    }
  }
  return {items,problems};
}
export function maskBulkIdentity(value: string | null): string {
  if (!value) return '—';
  return value.length > 4 ? '*'.repeat(value.length-4) + value.slice(-4) : '****';
}
