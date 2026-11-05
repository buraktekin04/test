import type { DispatchDetails, DispatchPerson } from '../models/dispatch.model';

/** Tek istekte sınırlı veri işleyerek SQL, bellek ve tarayıcı kaynaklarını koruruz. */
export const MAX_BULK_DISPATCH_ROWS = 100;
export const MAX_BULK_DISPATCH_FILE_BYTES = 1024 * 1024;

export interface BulkDispatchItem {
  person: DispatchPerson;
  dispatch: DispatchDetails;
  /** Eski MSKBS SenderChair/muayeneyeGonderenMakam; GUID değiştirilmeden backend'de türü belirlenir. */
  legacySenderChairId?: string | null;
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
  // Eski MSKBS dosyaları camelCase/PascalCase karışık olabilir; değeri asla yeniden üretme.
  for (const key of keys) if (Object.prototype.hasOwnProperty.call(row, key)) return row[key];
  for (const [actual, value] of Object.entries(row)) {
    if (keys.some(key => key.toLocaleLowerCase('tr-TR') === actual.toLocaleLowerCase('tr-TR')))
      return value;
  }
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
    // Eski MSKBS tek kayıt JSON dosyası, person/dispatch alt nesnelerine sahip değildir.
    if (pick(wrapper,'uniqueRefno','tcKimlikNo','foreignUniqueRefno','yabanciKimlikNo') !== null &&
        pick(wrapper,'HcAdmissionTypeDef','skBasvuruGrubu','hcAdmissionTypeDefId') !== null)
      return [wrapper];
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
    tcKimlikNo: text(pick(sourcePerson,'tcKimlikNo','TcKimlikNo','uniqueRefno','UniqueRefno')),
    yabanciKimlikNo: text(pick(sourcePerson,'yabanciKimlikNo','YabanciKimlikNo','foreignUniqueRefno','ForeignUniqueRefno')),
    name: text(pick(sourcePerson,'name','Name','adi','Adi')) ?? '',
    surname: text(pick(sourcePerson,'surname','Surname','soyadi','Soyadi')) ?? '',
    motherName: text(pick(sourcePerson,'motherName','MotherName','anneAdi','AnneAdi')),
    fatherName: text(pick(sourcePerson,'fatherName','FatherName','babaAdi','BabaAdi')),
    nationalityId: text(pick(sourcePerson,'nationalityId','NationalityId','nationality','Nationality','uyrugu','Uyrugu')),
    townOfBirthId: text(pick(sourcePerson,'townOfBirthId','TownOfBirthId','townOfBirth','TownOfBirth','dogumYeri','DogumYeri')),
    birthDate: date(pick(sourcePerson,'birthDate','BirthDate','dogumTarihi','DogumTarihi')) ?? '',
    gender: number(pick(sourcePerson,'gender','Gender','cinsiyet')),
    phoneNumber: text(pick(sourcePerson,'phoneNumber','PhoneNumber','telefonNo')),
    address: text(pick(sourcePerson,'address','Address','adres')),
    birthPlaceText: text(pick(sourcePerson,'birthPlaceText','BirthPlaceText','bdPlace','BdPlace')),
    birthPlaceCode: text(pick(sourcePerson,'birthPlaceCode','BirthPlaceCode','bdCode','BdCode'))
  };
  const scope = number(pick(sourceDispatch,'dispatchScope','DispatchScope','sevkKapsami','SevkKapsami')) ?? 0;
  let senderUnit = text(pick(sourceDispatch,'senderMilitaryUnitDefId','SenderMilitaryUnitDefId'));
  let senderOffice = text(pick(sourceDispatch,'senderMilitaryOfficeDefId','SenderMilitaryOfficeDefId'));
  const legacySender = text(pick(sourceDispatch,'legacySenderChairId','LegacySenderChairId',
    'senderChair','SenderChair','muayeneyeGonderenMakam','MuayeneyeGonderenMakam'));
  const senderChoice = text(pick(sourceDispatch,'senderChairChoice','SenderChairChoice'));
  const typedSender = /^(unit|office):([0-9a-f-]{36})$/i.exec(senderChoice ?? legacySender ?? '');
  let legacySenderChairId: string | null = legacySender;
  if (typedSender) {
    if (typedSender[1].toLowerCase() === 'unit') senderUnit = senderUnit ?? typedSender[2];
    else senderOffice = senderOffice ?? typedSender[2];
    if (legacySender && typedSender[0] === legacySender) legacySenderChairId = null;
  }
  const dispatch: DispatchDetails = {
    nthAdmission: number(pick(sourceDispatch,'nthAdmission','NthAdmission','kacinciIslem')),
    dispatchDocumentDate: date(pick(sourceDispatch,'dispatchDocumentDate','DispatchDocumentDate','sevkEvrakTarihi')),
    dispatchDocumentNo: text(pick(sourceDispatch,'dispatchDocumentNo','DispatchDocumentNo','sevkEvrakSayisi')),
    reasonForExaminationDefId: text(pick(sourceDispatch,'reasonForExaminationDefId','ReasonForExaminationDefId','muayeneSebebi','ReasonForExamination')),
    dispatchScope: scope,
    dispatchScopeCityId: text(pick(sourceDispatch,'dispatchScopeCityId','DispatchScopeCityId','sevkKapsamiSehir','DispatchScopeProvince')),
    dispatchScopeHospitalId: text(pick(sourceDispatch,'dispatchScopeHospitalId','DispatchScopeHospitalId','sevkKapsamiHastane','DispatchScopeHospital')),
    hcAdmissionTypeDefId: text(pick(sourceDispatch,'hcAdmissionTypeDefId','HcAdmissionTypeDefId','skBasvuruGrubu','HcAdmissionTypeDef')),
    forcesCommandDefId: text(pick(sourceDispatch,'forcesCommandDefId','ForcesCommandDefId','kuvvet','ForcesCommand')),
    rankDefId: text(pick(sourceDispatch,'rankDefId','RankDefId','rutbe','Rank')),
    militaryClassDefId: text(pick(sourceDispatch,'militaryClassDefId','MilitaryClassDefId','sinif','MilitaryClass')),
    militaryUnitDefId: text(pick(sourceDispatch,'militaryUnitDefId','MilitaryUnitDefId','birlik','MilitaryUnit')),
    militarySchoolDefId: text(pick(sourceDispatch,'militarySchoolDefId','MilitarySchoolDefId','okulu','MilitarySchool')),
    senderMilitaryUnitDefId: senderUnit,
    senderMilitaryOfficeDefId: senderOffice,
    senderChairOther: text(pick(sourceDispatch,'senderChairOther','SenderChairOther','muayeneyeGonderenMakamAciklama')),
    militaryOfficeDefId: text(pick(sourceDispatch,'militaryOfficeDefId','MilitaryOfficeDefId','kayitliOlduguAskerlikSubesi','MilitaryOffice')),
    flyingMissionDefId: text(pick(sourceDispatch,'flyingMissionDefId','FlyingMissionDefId','ucucuGorevi','FlyingMission')),
    planeTypeDefId: text(pick(sourceDispatch,'planeTypeDefId','PlaneTypeDefId','uctuguUcakTipi','FlyingPlaneType')),
    registryNumber: text(pick(sourceDispatch,'registryNumber','RegistryNumber','sicilNo')),
    lastNominationDate: date(pick(sourceDispatch,'lastNominationDate','LastNominationDate','nasbi')),
    militaryUnitDescription: text(pick(sourceDispatch,'militaryUnitDescription','MilitaryUnitDescription','birlikAciklama')),
    purchaseId: number(pick(sourceDispatch,'purchaseId','PurchaseId','alimId')),
    description: text(pick(sourceDispatch,'description','Description','Desciption'))
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
  const references: readonly [string, string | null][] = [
    ['Uyruk',person.nationalityId],
    ['Doğum yeri',person.townOfBirthId],
    ['Başvuru grubu',dispatch.hcAdmissionTypeDefId],
    ['Muayene sebebi',dispatch.reasonForExaminationDefId],
    ['Kapsam şehir',dispatch.dispatchScopeCityId],
    ['Kapsam hastane',dispatch.dispatchScopeHospitalId],
    ['Kuvvet',dispatch.forcesCommandDefId],
    ['Rütbe',dispatch.rankDefId],
    ['Askerî sınıf',dispatch.militaryClassDefId],
    ['Birlik',dispatch.militaryUnitDefId],
    ['Askerî okul',dispatch.militarySchoolDefId],
    ['Gönderen birlik',senderUnit],
    ['Gönderen askerlik şubesi',senderOffice],
    ['Kayıtlı askerlik şubesi',dispatch.militaryOfficeDefId],
    ['Uçucu görev',dispatch.flyingMissionDefId],
    ['Uçak tipi',dispatch.planeTypeDefId],
    ['Eski gönderen makam',legacySenderChairId]
  ];
  for (const [field, reference] of references) {
    if (reference !== null && !guid(reference)) {
      throw new Error(field + ' için dosyadaki değer geçerli bir GUID değildir. Eski ID değiştirilmeden aktarılmalıdır.');
    }
  }

  if (!guid(dispatch.hcAdmissionTypeDefId)) throw new Error('Başvuru grubu GUID kimliği gereklidir.');
  if (!guid(dispatch.reasonForExaminationDefId)) throw new Error('Muayene sebebi GUID kimliği gereklidir.');
  if (scope !== 2 && scope !== 3) throw new Error('Sevk kapsamı 2 (şehir) veya 3 (sağlık tesisi) olmalıdır.');
  if (scope === 2 && !guid(dispatch.dispatchScopeCityId)) throw new Error('Şehir kapsamı için şehir GUID kimliği gereklidir.');
  if (scope === 3 && !guid(dispatch.dispatchScopeHospitalId)) throw new Error('Sağlık tesisi kapsamı için hastane GUID kimliği gereklidir.');
  if (scope === 2 && dispatch.dispatchScopeHospitalId || scope === 3 && dispatch.dispatchScopeCityId)
    throw new Error('Sevk kapsamıyla ilgisiz şehir/hastane alanı boş bırakılmalıdır.');
  if (Number(!!senderUnit) + Number(!!senderOffice) + Number(!!legacySenderChairId) !== 1)
    throw new Error('Eski muayeneyeGonderenMakam/SenderChair veya yeni gönderen makam alanlarından tam olarak biri dolu olmalıdır.');
  // Dosyadan gelen tanım GUID'leri biçim dışında hiçbir dönüşüm görmez.
  return {person,dispatch,...(legacySenderChairId ? {legacySenderChairId} : {})};
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
