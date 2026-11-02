import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import {dirname,resolve} from 'node:path';
import {fileURLToPath} from 'node:url';

const frontend=resolve(dirname(fileURLToPath(import.meta.url)),'..');
const backend=resolve(frontend,'../Backend');
const read=(path)=>readFileSync(resolve(frontend,path),'utf8');
const readBackend=(path)=>readFileSync(resolve(backend,path),'utf8');

const diagnosis=read('src/app/features/syy-diagnoses/syy-diagnosis-list/syy-diagnosis-list.component.html');
const diagnosisTs=read('src/app/features/syy-diagnoses/syy-diagnosis-list/syy-diagnosis-list.component.ts');
const statistics=read('src/app/features/syy-diagnosis-decisions/syy-diagnosis-decision-list/syy-diagnosis-decision-list.component.html');
const statisticsScss=read('src/app/features/syy-diagnosis-decisions/syy-diagnosis-decision-list/syy-diagnosis-decision-list.component.scss');
const statisticsTs=read('src/app/features/syy-diagnosis-decisions/syy-diagnosis-decision-list/syy-diagnosis-decision-list.component.ts');
const classification=read('src/app/features/syy-classifications/syy-classification-list/syy-classification-list.component.html');
const classificationTs=read('src/app/features/syy-classifications/syy-classification-list/syy-classification-list.component.ts');
const classificationService=read('src/app/features/syy-classifications/services/syy-classification.service.ts');
const sharedStyles=read('src/styles.scss');
const appConfig=read('src/app/app.config.ts');
const query=readBackend('KLMN.Application/SYYClassifications/Queries/GetSYYClassificationDefOptions/GetSYYClassificationDefOptionsQuery.cs');
const handler=readBackend('KLMN.Application/SYYClassifications/Queries/GetSYYClassificationDefOptions/GetSYYClassificationDefOptionsQueryHandler.cs');
const controller=readBackend('KLMN.Api/Controllers/SYYClassificationsController.cs');
const rule=readBackend('KLMN.Application/SYYClassifications/Commands/Common/SYYClassificationDefRules.cs');

for(const key of ['attachedRecNo','syyDiagnosisType','slice']){
 assert.match(diagnosis,new RegExp('<p-select[^>]*formControlName="'+key+'"'));
 assert.doesNotMatch(diagnosis,new RegExp('<select[^>]*formControlName="'+key+'"'));
}
assert.match(diagnosisTs,/SelectModule/);
assert.match(diagnosis,/Bağlı kayıtlarda ara/);

for(const key of ['syyDiagnosisDefId','hcDecisionDefId','hcAdmissionTypeDefId','militaryClassDefId']){
 assert.match(statistics,new RegExp('<p-select[^>]*formControlName="'+key+'"'));
 assert.doesNotMatch(statistics,new RegExp('<select[^>]*formControlName="'+key+'"'));
}
assert.match(statisticsTs,/SelectModule/);
assert.match(statistics,/filterPlaceholder="Kurul kararlarında ara/);
assert.match(statistics,/class="wrap syy-diagnosis-cell"/);
assert.match(statisticsScss,/td\.syy-diagnosis-cell\{/);
assert.match(statisticsScss,/white-space:normal!important/);

for(const key of ['syyDiagnosisDefId','forcesCommandDef','rankGroup','militaryClassGroup','hcDecision']){
 assert.match(classification,new RegExp('<p-select[^>]*formControlName="'+key+'"'));
}
assert.match(classificationTs,/SelectModule/);
assert.match(classification,/\[disabled\]="!form\.controls\.forcesCommandDef\.value"/);
assert.match(classification,/Önce kuvvet komutanlığını seçiniz/);
assert.match(classificationTs,/this\.form\.controls\.militaryClassGroup\.setValue\(''\)/);
assert.match(classificationTs,/optionsRequestId/);
assert.match(classificationService,/filterForcesCommandDef/);
assert.match(query,/Guid\? FilterForcesCommandDef=null/);
assert.match(controller,/filterForcesCommandDef/);
assert.match(handler,/x\.ForcesCommandId==r\.FilterForcesCommandDef\.Value/);
assert.match(rule,/Seçilen sınıf grubu, seçilen kuvvet komutanlığına bağlı olmalıdır/);

assert.match(sharedStyles,/\.syy-search-overlay\.p-select-overlay/);
assert.match(sharedStyles,/\.syy-select-option-text/);
assert.match(sharedStyles,/white-space:normal/);
assert.match(appConfig,/filters\.register\('contains',containsIgnoringCase\)/);

console.log('PASS: Aranabilir SYY açılır seçimler, çok satırlı karar/tanı metinleri ve kuvvete bağlı sınıf grupları.');
