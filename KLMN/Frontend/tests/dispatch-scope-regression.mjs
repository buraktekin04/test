import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const root=resolve(dirname(fileURLToPath(import.meta.url)),'../..');
const read=p=>readFileSync(resolve(root,p),'utf8');
const ts=read('Frontend/src/app/features/dispatches/dispatch-entry/dispatch-entry.component.ts');
const html=read('Frontend/src/app/features/dispatches/dispatch-entry/dispatch-entry.component.html');
const input=read('Backend/KLMN.Application/Dispatches/Models/DispatchDetailsInput.cs');
const rules=read('Backend/KLMN.Application/Dispatches/Commands/Common/DispatchRules.cs');
const create=read('Backend/KLMN.Application/Dispatches/Commands/CreateDispatch/CreateDispatchCommandValidator.cs');
const update=read('Backend/KLMN.Application/Dispatches/Commands/UpdateDispatch/UpdateDispatchCommandValidator.cs');
const history=read('Frontend/src/app/features/dispatches/dispatch-list/dispatch-list.component.ts');
const historyHtml=read('Frontend/src/app/features/dispatches/dispatch-list/dispatch-list.component.html');
const queryValidator=read('Backend/KLMN.Application/Dispatches/Queries/GetDispatches/GetDispatchesQueryValidator.cs');
const db=read('Backend/KLMN.Persistence/Configurations/Admissions/DispatchConfiguration.cs');

assert.match(ts,/dispatchScope:this\.fb\.control<number\|null>\(null,Validators\.required\)/);
assert.match(ts,/this\.dispatchForm\.reset\(\{dispatchScope:null,/);
assert.match(ts,/readonly scopeItems=\[\{label:'Şehirdeki sağlık tesisleri',value:2\},\s*\{label:'Belirli sağlık tesisi',value:3\}\]/);
assert.doesNotMatch(ts,/Kapsam belirtilmemiş/);
assert.match(ts,/if\(dispatch\.dispatchScope!==2&&dispatch\.dispatchScope!==3\)/);
assert.match(ts,/this\.formError\.set\('Sevk kapsamı seçiniz\.'/);
assert.match(html,/<span class="required-mark">\*<\/span> Sevk Kapsamı/);
assert.match(html,/placeholder="Sevk kapsamı seçiniz"/);
assert.match(html,/@if\(dispatchForm\.controls\.dispatchScope\.value===2\)/);
assert.match(html,/@if\(dispatchForm\.controls\.dispatchScope\.value===3\)/);
assert.match(input,/public int DispatchScope \{ get; set; \}/);
assert.doesNotMatch(input,/DispatchScope \{ get; set; \} = 1;/);
assert.match(rules,/if \(d\.DispatchScope is not \(2 or 3\)\)/);
for(const v of [create,update]){
 assert.match(v,/Dispatch!\.DispatchScope\)\.Must\(scope=>scope is 2 or 3\)/);
 assert.match(v,/Dispatch!\.DispatchScopeCityId\)\.NotNull\(\)/);
 assert.match(v,/Dispatch!\.DispatchScopeHospitalId\)\.NotNull\(\)/);
}
// Sorgu filtresi isteğe bağlı, fakat açık bir kapsam seçilirse sadece 2/3 geçerli.
assert.match(history,/readonly scopeItems=\[\s*\{label:'Şehirdeki sağlık tesisleri',value:2\},\s*\{label:'Belirli sağlık tesisi',value:3\}\s*\]/);
assert.doesNotMatch(history,/Kapsam belirtilmemiş/);
assert.match(history,/dispatchScope:this\.fb\.control<number\|null>\(null\)/);
assert.match(history,/dispatchScope:null,dispatchScopeCityId:null,dispatchScopeHospitalId:null/);
assert.match(historyHtml,/formControlName="dispatchScope"[\s\S]*?placeholder="Tüm kapsamlar"/);
assert.match(historyHtml,/\[showClear\]="true"/);
assert.match(queryValidator,/RuleFor\(x=>x\.DispatchScope\)\.Must\(x=>!x\.HasValue\|\|\(x\.Value is 2 or 3\)\)/);
assert.match(db,/CK_Dispatch_Scope/);
console.log('PASS: Sevk girişinde kapsam 2/3 zorunlu; liste filtresinde 2/3 veya boş (tarihsel tüm kayıtlar).');
