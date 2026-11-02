import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import {dirname,resolve} from 'node:path';
import {fileURLToPath} from 'node:url';

const frontend=resolve(dirname(fileURLToPath(import.meta.url)),'..');
const backend=resolve(frontend,'../Backend');
const read=path=>readFileSync(resolve(frontend,path),'utf8');
const be=path=>readFileSync(resolve(backend,path),'utf8');

const component=read('src/app/features/syy-diagnosis-requireds/syy-diagnosis-required-list/syy-diagnosis-required-list.component.ts');
const template=read('src/app/features/syy-diagnosis-requireds/syy-diagnosis-required-list/syy-diagnosis-required-list.component.html');
const css=read('src/app/features/syy-diagnosis-requireds/syy-diagnosis-required-list/syy-diagnosis-required-list.component.scss');
const rulesView=read('src/app/features/syy-rules/syy-rule-list/syy-rule-list.component.html');
const rules=be('KLMN.Application/SYYDiagnosisRequireds/Commands/Common/SYYDiagnosisRequiredRules.cs');
const createValidator=be('KLMN.Application/SYYDiagnosisRequireds/Commands/CreateSYYDiagnosisRequired/CreateSYYDiagnosisRequiredCommandValidator.cs');
const updateValidator=be('KLMN.Application/SYYDiagnosisRequireds/Commands/UpdateSYYDiagnosisRequired/UpdateSYYDiagnosisRequiredCommandValidator.cs');
const createHandler=be('KLMN.Application/SYYDiagnosisRequireds/Commands/CreateSYYDiagnosisRequired/CreateSYYDiagnosisRequiredCommandHandler.cs');
const updateHandler=be('KLMN.Application/SYYDiagnosisRequireds/Commands/UpdateSYYDiagnosisRequired/UpdateSYYDiagnosisRequiredCommandHandler.cs');
const entity=be('KLMN.Domain/Entities/Admissions/SYYDiagnosisRequired.cs');

// UI must have one RadioButton reactive form control and no independent flags.
assert.match(component,/import \{RadioButtonModule\} from 'primeng\/radiobutton'/);
assert.match(component,/ruleCondition:this\.fb\.control<'all'\|'one'\|null>\(null,Validators\.required\)/);
assert.doesNotMatch(component,/mustAll:this\.fb\.control/);
assert.doesNotMatch(component,/mustOneOf:this\.fb\.control/);
assert.match(template,/formControlName="ruleCondition" value="all"/);
assert.match(template,/formControlName="ruleCondition" value="one"/);
assert.match(template,/name="ruleCondition"/);
assert.match(template,/role="radiogroup"/);
assert.doesNotMatch(template,/formControlName="mustAll"/);
assert.doesNotMatch(template,/formControlName="mustOneOf"/);
assert.match(css,/\.rule-condition-options\{/);
assert.match(css,/\.rule-condition-option\{/);

// Create/update are normalized to mutually exclusive booleans on the existing API.
assert.match(component,/ruleCondition:null/);
assert.match(component,/x\.mustAll===true&&x\.mustOneOf===false\?'all'/);
assert.match(component,/x\.mustOneOf===true&&x\.mustAll===false\?'one':null/);
assert.match(component,/mustAll:f\.ruleCondition==='all',mustOneOf:f\.ruleCondition==='one'/);
assert.match(template,/Kural Koşulu/);
assert.match(template,/Koşul seçilmemiş/);
assert.match(rulesView,/Kural koşulu seçilmemiş \/ geçersiz/);

// Both validators AND handlers reject none/both even when bypassing UI.
assert.match(rules,/HasExactlyOneCondition\(bool\? mustAll,bool\? mustOneOf\)/);
assert.match(rules,/mustAll==true && mustOneOf==false/);
assert.match(rules,/mustAll==false && mustOneOf==true/);
assert.match(rules,/EnsureExclusiveCondition/);
assert.match(createValidator,/SYYDiagnosisRequiredRules\.HasExactlyOneCondition/);
assert.match(updateValidator,/SYYDiagnosisRequiredRules\.HasExactlyOneCondition/);
assert.match(createHandler,/SYYDiagnosisRequiredRules\.EnsureExclusiveCondition/);
assert.match(updateHandler,/SYYDiagnosisRequiredRules\.EnsureExclusiveCondition/);

// Existing DB flags remain in place, no migration/column deletion necessary.
assert.match(entity,/bool\? MustAll/);
assert.match(entity,/bool\? MustOneOf/);

console.log('PASS: SYY onay kuralları radio seçimi create/update/UI/API üzerinde tam tek koşul.');
