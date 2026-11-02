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

console.log('PASS: ICD10 seçenekleri aynı tek API çağrısından gelir; JSON icd10s eşleşir, 3 dropdown aranabilir.');
