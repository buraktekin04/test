/**
 * Eski MSKBS Sevk Giriş QR metnini güvenli bir alan beyaz listesi ile çözümler.
 * Format: tcKimlikNo:123...,dogumTarihi:01.02.2000,sevkEvrakTarihi:...
 * Bilinmeyen alanlar asla dinamik olarak forma yazılmaz.
 */
export interface DispatchQrData {
  tcKimlikNo?: string;
  yabanciKimlikNo?: string;
  dogumTarihi?: string;
  sevkEvrakTarihi?: string;
  sevkEvrakSayisi?: string;
  muayeneSebebi?: string;
  sevkKapsamiHastane?: string;
  alimId?: number;
  kacinciIslem?: number;
  telefonNo?: string;
}

export class DispatchQrFormatError extends Error {
  constructor(message: string) {
    super(message);
    this.name = 'DispatchQrFormatError';
  }
}

const GUID = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-8][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
const FIELDS = new Set(['tcKimlikNo','yabanciKimlikNo','dogumTarihi',
  'sevkEvrakTarihi','sevkEvrakSayisi','muayeneSebebi',
  'sevkKapsamiHastane','alimId','kacinciIslem','telefonNo']);

/** 31.02 gibi geçersiz tarihleri de reddedip yyyy-MM-dd döndürür. */
function parseDate(value: string, field: string): string {
  const input = value.split(' ')[0].trim();
  const match = /^(\d{2})\.(\d{2})\.(\d{4})$/.exec(input);
  const iso = /^(\d{4})-(\d{2})-(\d{2})$/.exec(input);
  if (!match && !iso) throw new DispatchQrFormatError(field+' tarihi GG.AA.YYYY veya YYYY-AA-GG olmalıdır.');
  const year=Number(match?match[3]:iso![1]);
  const month=Number(match?match[2]:iso![2]);
  const day=Number(match?match[1]:iso![3]);
  const date=new Date(Date.UTC(year,month-1,day));
  if(date.getUTCFullYear()!==year||date.getUTCMonth()!==month-1||date.getUTCDate()!==day||
    year<1900 || year>2100)
    throw new DispatchQrFormatError(field+' geçersiz bir tarih içeriyor.');
  return [year.toString().padStart(4,'0'),String(month).padStart(2,'0'),String(day).padStart(2,'0')].join('-');
}

/** Eski CSV-benzeri karekod ve cihazlardan gelen baş/son çift tırnakları destekler. */
function unpack(raw: string): Record<string,string> {
  if (!raw.trim()) throw new DispatchQrFormatError('QR kod boş olamaz.');
  if (raw.length>4096) throw new DispatchQrFormatError('QR kod 4096 karakteri geçemez.');
  if (raw.trimStart().startsWith('{')) {
    let obj: unknown;
    try { obj=JSON.parse(raw); } catch { throw new DispatchQrFormatError('QR JSON biçimi geçersiz.'); }
    if (!obj || typeof obj!=='object' || Array.isArray(obj)) throw new DispatchQrFormatError('QR nesne olmalıdır.');
    const values: Record<string,string>={};
    for (const [key,value] of Object.entries(obj)) {
      if(FIELDS.has(key)&&value!==null&&value!==undefined)values[key]=String(value);
    }
    return values;
  }
  const values: Record<string,string>={};
  const parts=raw.replace(/[\r\n]+/g,',').split(',');
  for (const part of parts) {
    const index=part.indexOf(':');
    if(index<0)continue;
    const key=part.slice(0,index).trim().replace(/^"|"$/g,'');
    if(!FIELDS.has(key))continue;
    const value=part.slice(index+1).trim().replace(/^"|"$/g,'');
    if (key in values) throw new DispatchQrFormatError('QR içinde yinelenen alan: '+key);
    values[key]=value;
  }
  return values;
}

/** Tüm alanları kullanıcı formuna yazmadan önce tip ve uzunluklarını doğrular. */
export function parseDispatchQr(raw: string): DispatchQrData {
  const data=unpack(raw);
  if(Object.keys(data).length===0)throw new DispatchQrFormatError('QR kodda bilinen sevk alanı bulunamadı.');
  const result:DispatchQrData={};
  if('tcKimlikNo' in data) {
    if(!/^\d{11}$/.test(data.tcKimlikNo))throw new DispatchQrFormatError('T.C. kimlik no 11 rakam olmalıdır.');
    result.tcKimlikNo=data.tcKimlikNo;
  }
  if('yabanciKimlikNo' in data) {
    if(!/^\d{11,20}$/.test(data.yabanciKimlikNo))
      throw new DispatchQrFormatError('Yabancı kimlik no 11-20 rakam olmalıdır.');
    result.yabanciKimlikNo=data.yabanciKimlikNo;
  }
  if(result.tcKimlikNo && result.yabanciKimlikNo)
    throw new DispatchQrFormatError('QR içinde hem T.C. hem yabancı kimlik numarası olamaz.');
  if(data.dogumTarihi)result.dogumTarihi=parseDate(data.dogumTarihi,'Doğum');
  if(data.sevkEvrakTarihi)result.sevkEvrakTarihi=parseDate(data.sevkEvrakTarihi,'Sevk evrak');
  if(data.sevkEvrakSayisi) {
    if(data.sevkEvrakSayisi.length>50)throw new DispatchQrFormatError('Sevk evrak sayısı 50 karakteri aşamaz.');
    result.sevkEvrakSayisi=data.sevkEvrakSayisi;
  }
  for(const key of ['muayeneSebebi','sevkKapsamiHastane'] as const) {
    if(data[key]) {
      if(!GUID.test(data[key]))throw new DispatchQrFormatError(key+' alanı geçerli GUID olmalıdır.');
      result[key]=data[key].toLowerCase();
    }
  }
  for (const [key,value] of [['alimId','Alım ID'],['kacinciIslem','Kaçıncı işlem']] as const) {
    if(data[key]) {
      if(!/^\d{1,9}$/.test(data[key]))throw new DispatchQrFormatError(value+' geçerli bir sayı olmalıdır.');
      const n=Number(data[key]);
      if(key==='kacinciIslem'&&n<1)throw new DispatchQrFormatError('Kaçıncı işlem 1 veya daha büyük olmalıdır.');
      result[key]=n;
    }
  }
  if(data.telefonNo) {
    const phone=data.telefonNo.replace(/\s/g,'');
    if(!/^\d{10}$/.test(phone))throw new DispatchQrFormatError('Telefon numarası 10 haneli olmalıdır.');
    result.telefonNo=phone;
  }
  return result;
}
