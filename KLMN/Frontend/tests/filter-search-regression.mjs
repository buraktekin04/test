import assert from 'node:assert/strict';
import {
 containsIgnoringCase,startsWithIgnoringCase,endsWithIgnoringCase,normalizeFilterText
} from '../src/app/core/filters/case-insensitive-filter.ts';

// Aynı arama sonuçları büyük/küçük harf ve Türkçe noktalı/noktasız I ile değişmez.
for(const [value,term] of [
 ['İSTANBUL','istanbul'],['istanbul','İSTANBUL'],['ISPARTA','ısparta'],
 ['KIRIKKALE','kırıkkale'],['İlçe Tanımları','ilçe'],['ANKARA','ankara'],
 ['MUAYENE SEBEBİ','sebebi'],['ERKEK','erkek'],['Adana','ADA']
]){
 assert.equal(containsIgnoringCase(value,term),true,value+' ~ '+term);
}
assert.equal(containsIgnoringCase('Aydın','zonguldak'),false);
assert.equal(containsIgnoringCase(null,'ankara'),false);
assert.equal(containsIgnoringCase('Ankara',''),true);
assert.equal(startsWithIgnoringCase('ŞEHİR','şeh'),true);
assert.equal(endsWithIgnoringCase('Askerî Okul','OKUL'),true);
assert.equal(normalizeFilterText('İIıi'),'iiii');
console.log('PASS: Global PrimeNG Turkish case-insensitive contains/startsWith/endsWith.');
