/**
 * PrimeNG dropdown/table filtrelemesinde Türkçe dahil harf büyüklüğüne duyarsız
 * karşılaştırma. "İ", "I", "ı" ve "i" aynı arama karakterine indirgenir.
 * Yazı işaretleri de normalize edilir, boş sorgu her değerle eşleşir.
 */
export function normalizeFilterText(value:unknown):string {
 if(value===null||value===undefined)return '';
 return String(value)
  .normalize('NFD')
  .replace(/[\u0300-\u036f]/g,'')
  .toLocaleLowerCase('tr-TR')
  .replace(/ı/g,'i');
}

export function containsIgnoringCase(value:unknown,filter:unknown):boolean {
 if(filter===undefined||filter===null||String(filter).trim()==='')return true;
 if(value===undefined||value===null)return false;
 return normalizeFilterText(value).includes(normalizeFilterText(filter));
}
