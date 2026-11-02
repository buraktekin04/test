import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import {dirname,resolve} from 'node:path';
import {fileURLToPath} from 'node:url';

const frontend=resolve(dirname(fileURLToPath(import.meta.url)),'..');
const root=resolve(frontend,'..');
const read=path=>readFileSync(resolve(root,path),'utf8');

const old=read('../test');
const ts=read('Frontend/src/app/features/dispatches/dispatch-entry/dispatch-entry.component.ts');
const html=read('Frontend/src/app/features/dispatches/dispatch-entry/dispatch-entry.component.html');
const model=read('Frontend/src/app/features/dispatches/models/dispatch.model.ts');
const response=read('Backend/KLMN.Application/Dispatches/Responses/DispatchAdmissionRulesResponse.cs');
const handler=read('Backend/KLMN.Application/Dispatches/Queries/GetDispatchAdmissionRules/GetDispatchAdmissionRulesQueryHandler.cs');
const options=read('Backend/KLMN.Application/Dispatches/Queries/GetDispatchOptions/GetDispatchOptionsQueryHandler.cs');
const optionDto=read('Backend/KLMN.Application/Dispatches/Responses/DispatchOption.cs');
const validation=read('Backend/KLMN.Application/Dispatches/Commands/Common/DispatchRules.cs');

for(const method of [
 'onHcAdmissinTypeValueChanged','getReasonForExaminationListByHcAdmissionType',
 'onSelectionChangedMilitaryUnit','onSelectionChanged(event: any)'
]) assert.ok(old.includes(method),'Eski metot bulunamadı: '+method);

// Başvuru grubu değişimi ile zorunlu alanlar, kullanılmayan sebepler, izinli kuvvetler.
assert.match(ts,/hcAdmissionTypeDefId\.valueChanges\.subscribe/);
assert.match(ts,/loadAdmissionRules\(id,true\)/);
assert.match(ts,/if\(groupChanged\)/);
assert.match(ts,/isGroupFieldEnabled\(key:string\)/);
assert.match(ts,/this\.loadOptions\('reasons'/);
assert.match(ts,/unusedReasonIds/);
assert.match(ts,/visibleOptions\(type:OptionType\)/);
assert.match(ts,/allowedForcesCommandIds/);

for(const key of [
 'forcesCommandDefId','rankDefId','militaryClassDefId','militaryUnitDefId',
 'militaryOfficeDefId','militarySchoolDefId','registryNumber',
 'lastNominationDate','nthAdmission','dispatchDocumentDate','dispatchDocumentNo'
]) assert.ok(ts.includes(key+':r.'),'Eksik gerekli alan: '+key);

assert.match(html,/\[options\]="visibleOptions\(field.type\)"/);
assert.match(html,/\[disabled\]="!isGroupFieldEnabled\(field.key\)"/);
assert.match(html,/\[disabled\]="!isGroupFieldEnabled\('registryNumber'\)"/);
assert.match(html,/\[disabled\]="!isGroupFieldEnabled\('nthAdmission'\)"/);
assert.match(html,/\[disabled\]="!isGroupFieldEnabled\('dispatchDocumentDate'\)"/);
assert.match(html,/\[disabled\]="!isGroupFieldEnabled\('dispatchDocumentNo'\)"/);
assert.match(html,/\[disabled\]="!isGroupFieldEnabled\('lastNominationDate'\)"/);

// Kod 7 tüm kuvvetler, 33 yalnız 1/2/3, diğer gruplar 4 ve 5 hariç.
// Kurallar server tarafında da uygulandığı için API dışından bypass edilemez.
for(const code of ['group.Code==7','group.Code==33','x.Code=="1"','x.Code=="2"',
 'x.Code=="3"','x.Code!="4"','x.Code!="5"']){
 assert.ok(handler.includes(code),'Option policy eksik: '+code);
 assert.ok(validation.includes(code),'Server policy eksik: '+code);
}
assert.match(response,/AllowedForcesCommandIds/);
assert.match(model,/allowedForcesCommandIds:string\[\]/);
assert.match(optionDto,/string\? Code=null/);
for(const type of ['forces','units','offices']){
 const index=options.indexOf('"'+type+'" =>');
 assert.ok(index>=0);
 const segment=options.slice(index,options.indexOf('ToListAsync(ct)',index));
 assert.ok(segment.includes('new DispatchOption(x.Id,x.Name,x.Code)'),type+' code eksik');
}

// DİĞER seçimlerinde açıklama zorunlu; sadece frontend'te değil backend'te de.
assert.match(ts,/isOtherMilitaryUnit\(\)/);
assert.match(ts,/isOtherSender\(\)/);
assert.match(html,/\@if\(isOtherMilitaryUnit\(\)\)/);
assert.match(html,/\@if\(isOtherSender\(\)\)/);
assert.match(validation,/Birlik DİĞER seçildiğinde Birlik Açıklama zorunludur/);
assert.match(validation,/Gönderen makam DİĞER seçildiğinde Gönderen Makam Açıklama zorunludur/);

// Düzenleme formu açılınca grup değişimi tetiklenip kaydedilmiş alanlar silinmez.
assert.match(ts,/this\.dispatchForm\.reset\(item\.dispatch,\{emitEvent:false\}\)/);
assert.match(ts,/this\.loadAdmissionRules\(item\.dispatch\.hcAdmissionTypeDefId,false\)/);
console.log('PASS: Eski Sevk Girişi başvuru grubu ve DİĞER davranışları KLMN frontend/backend sözleşmesinde.');
