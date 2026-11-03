import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import {dirname,resolve} from 'node:path';
import {fileURLToPath} from 'node:url';

const frontend=resolve(dirname(fileURLToPath(import.meta.url)),'..');
const read=path=>readFileSync(resolve(frontend,path),'utf8');
const be=path=>readFileSync(resolve(frontend,'../Backend',path),'utf8');
const ts=read('src/app/features/dispatches/dispatch-entry/dispatch-entry.component.ts');
const html=read('src/app/features/dispatches/dispatch-entry/dispatch-entry.component.html');
const model=read('src/app/features/dispatches/models/dispatch.model.ts');
const options=be('KLMN.Application/Dispatches/Queries/GetDispatchOptions/GetDispatchOptionsQueryHandler.cs');
const dto=be('KLMN.Application/Dispatches/Responses/DispatchOption.cs');
const rules=be('KLMN.Application/Dispatches/Commands/Common/DispatchRules.cs');

// Eski militaryunitandoffice, artık birleşik tek açılır liste.
assert.match(html,/formControlName="senderChairChoice"/);
assert.match(html,/\[options\]="senderOptions\(\)"/);
assert.match(html,/optionValue="choice"/);
assert.match(html,/filterOptions\('senders',\$event\.filter\)/);
assert.doesNotMatch(html,/formControlName="senderMilitaryUnitDefId"/);
assert.doesNotMatch(html,/formControlName="senderMilitaryOfficeDefId"/);
assert.match(ts,/senderChairChoice:this\.fb\.control<string\|null>\(null,Validators\.required\)/);
assert.match(ts,/senderOptions=computed/);
assert.match(ts,/x\.kind==='unit'\|\|x\.kind==='office'/);
assert.match(ts,/senderChairChoice\.valueChanges\.subscribe/);
assert.match(ts,/this\.auth\.currentUser\(\)\?\.militaryUnitDefId/);
assert.match(ts,/private prefillCurrentUserSender\(\)/);
assert.match(ts,/this\.prefillCurrentUserSender\(\)/);
assert.match(ts,/if\(type==='senders'&&!this\.senderDefaultChecked&&!this\.editing\(\)\)/);

assert.match(ts,/senderMilitaryUnitDefId:match\?\.\[1\]==='unit'\?match\[2\]:null/);
assert.match(ts,/senderMilitaryOfficeDefId:match\?\.\[1\]==='office'\?match\[2\]:null/);
assert.match(ts,/delete dispatch\.senderChairChoice/);
assert.match(ts,/this\\.dispatchForm\\.controls\\.senderChairChoice\\.setValue\\(senderChoice,\\{emitEvent:false\\}\\)/);
assert.ok(ts.includes("this.loadOptions('senders','',senderChoice.split(':')[1])"));
// Eski formun DİĞER -> açıklama kuralı korunur.
assert.match(ts,/isOtherSender\(\):boolean/);
assert.match(html,/\@if\(isOtherSender\(\)\)/);
assert.match(rules,/Gönderen makam DİĞER seçildiğinde Gönderen Makam Açıklama zorunludur/);

// Birlik/Şube aynı seçenek API'sine toplanır, discriminator kaybolmaz.
assert.match(model,/\|'senders'\|/);
assert.match(model,/kind\?:'unit'\|'office'/);
assert.match(dto,/string\? Kind=null/);
assert.match(options,/if\(request\.Type=="senders"\)/);
assert.match(options,/db\.MilitaryUnitDefs\.IgnoreQueryFilters\(\)/);
assert.match(options,/db\.MilitaryOfficeDefs\.IgnoreQueryFilters\(\)/);
assert.match(options,/new DispatchOption\(x\.Id,"Birlik · "\+x\.Name,x\.Code,"unit"\)/);
assert.match(options,/new DispatchOption\(x\.Id,"Askerlik Şubesi · "\+x\.Name,x\.Code,"office"\)/);

// Eski controlOfRequiredFiles: makam her zaman zorunlu, tek FK gerekir.
assert.match(rules,/Muayeneye Gönderen Makam boş geçilemez/);
assert.match(rules,/d\.SenderMilitaryUnitDefId\.HasValue && d\.SenderMilitaryOfficeDefId\.HasValue/);
assert.doesNotMatch(rules,/group\.SenderChairRequired && !d\.SenderMilitaryUnitDefId/);
console.log('PASS: Gönderen makam birleşik dropdown, FK mapping, DİĞER ve zorunluluk.');
