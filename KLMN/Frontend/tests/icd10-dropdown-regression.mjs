import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import {dirname,resolve} from 'node:path';
import {fileURLToPath} from 'node:url';

const frontend=resolve(dirname(fileURLToPath(import.meta.url)),'..');
const backend=resolve(frontend,'../Backend');
const read=path=>readFileSync(resolve(frontend,path),'utf8');
const be=path=>readFileSync(resolve(backend,path),'utf8');

const html=read('src/app/features/icd10-diagnostics/icd10-diagnostic-list/icd10-diagnostic-list.component.html');
const ts=read('src/app/features/icd10-diagnostics/icd10-diagnostic-list/icd10-diagnostic-list.component.ts');
const client=read('src/app/features/icd10-diagnostics/services/icd10-diagnostic.service.ts');
const model=read('src/app/features/icd10-diagnostics/models/icd10-diagnostic.model.ts');
const dto=be('KLMN.Application/ICD10Diagnostics/Queries/GetICD10DiagnosticOptions/GetICD10DiagnosticOptionsQuery.cs');
const listResponse=be('KLMN.Application/ICD10Diagnostics/Responses/ICD10DiagnosticResponse.cs');
const listHandler=be('KLMN.Application/ICD10Diagnostics/Queries/GetICD10Diagnostics/GetICD10DiagnosticsQueryHandler.cs');
const detailHandler=be('KLMN.Application/ICD10Diagnostics/Queries/GetICD10DiagnosticById/GetICD10DiagnosticByIdQueryHandler.cs');
const handler=be('KLMN.Application/ICD10Diagnostics/Queries/GetICD10DiagnosticOptions/GetICD10DiagnosticOptionsQueryHandler.cs');
const controller=be('KLMN.Api/Controllers/ICD10DiagnosticsController.cs');
const sharedCss=read('src/styles.scss');

// Three reference dropdowns are locally filtered PrimeNG selects with the existing API.
for(const [control,field] of [
 ['icd10DefId','icd10s'],
 ['syyDiagnosisDefId','syyDiagnoses'],
 ['hcAdmissionTypeDefId','admissionTypes']
]){
 const start=html.indexOf('<p-select',html.indexOf('id="icd10-diagnostic-'+(field==='icd10s'?'code':field==='syyDiagnoses'?'syy':'group')+'"')-200);
 const match=html.match(new RegExp('<p-select[^>]*formControlName="'+control+'"[\\s\\S]*?<\\/p-select>'));
 assert.ok(match,'Missing dropdown '+control);
 const tag=match[0];
 assert.ok(tag.includes('[options]="optionsData().'+field+'"'),'Wrong options '+field);
 for(const prop of ['optionLabel="label"','optionValue="id"','[filter]="true"','filterBy="label"','filterMatchMode="contains"','appendTo="body"']){
  assert.ok(tag.includes(prop),control+' missing '+prop);
 }
 assert.ok(!tag.includes('#selectedItem'),control+' custom selectedItem must not shrink label');
}
assert.match(ts,/import \{SelectModule\} from 'primeng\/select'/);
assert.match(ts,/this\.api\.options\(row\)/);
assert.match(client,/this\.http\.get<Icd10DiagnosticOptions>\(this\.url\+'\/options'/);
assert.match(model,/icd10s:Icd10DiagnosticOption\[\]/);
assert.match(dto,/JsonPropertyName\("icd10s"\)/);
assert.match(dto,/JsonPropertyName\("syyDiagnoses"\)/);
assert.match(dto,/JsonPropertyName\("admissionTypes"\)/);
assert.match(handler,/db\.ICD10Defs\.IgnoreQueryFilters\(\)\.AsNoTracking\(\)/);
assert.doesNotMatch(handler,/Take\(100\)|Skip\(/); // all 5 definitions, no paged ICD10 lookup
assert.match(controller,/\[HttpGet\("options"\)\]/);
assert.match(sharedCss,/flex:1 1 auto/);
assert.match(sharedCss,/width:auto!important/);
assert.doesNotMatch(sharedCss,/flex:1 1 0;/);
assert.doesNotMatch(sharedCss,/width:0;/);

// Liste/detail response JSON adları, Angular tablosu ve EF projection tutarlı olmalı.
for(const [jsonKey,clrKey] of [
 ['icd10DefId','ICD10DefId'],
 ['icd10Code','ICD10Code'],
 ['icd10Name','ICD10Name'],
 ['syyDiagnosisName','SYYDiagnosisName'],
 ['hcAdmissionTypeName','HcAdmissionTypeName'],
 ['amount','Amount'],
 ['isActive','IsActive']
]){
 assert.ok(listResponse.includes('[property: JsonPropertyName("'+jsonKey+'")]'),
  'Response JSON alanı eksik: '+jsonKey);
 assert.ok(listResponse.includes(clrKey),'CLR alanı eksik: '+clrKey);
}
assert.ok(html.includes('{{row.icd10Code}}'),'ICD10 Kodu kolon bağı');
assert.ok(html.includes('{{row.icd10Name}}'),'ICD10 Adı kolon bağı');
for(const source of [listHandler,detailHandler]){
 assert.ok(source.includes('new ICD10DiagnosticResponse('),'Ortak DTO projeksiyonu');
 assert.ok(source.includes('x.ICD10.Code'),'ICD10 ilişkisi kod projeksiyonu');
 assert.ok(source.includes('x.ICD10.Name'),'ICD10 ilişkisi ad projeksiyonu');
}
console.log('PASS: ICD10 kod/ad liste ve detay JSON alanları ile frontend kolonları eşleşir.');
