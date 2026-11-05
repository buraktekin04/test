import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import {dirname,resolve} from 'node:path';
import {fileURLToPath} from 'node:url';

const frontend=resolve(dirname(fileURLToPath(import.meta.url)),'..');
const source=(name)=>readFileSync(resolve(frontend,name),'utf8');
const backend=(name)=>source('../Backend/'+name);

const routes=source('src/app/app.routes.ts');
const nav=source('src/app/layout/services/navigation.service.ts');
const service=source('src/app/features/dispatches/services/dispatch.service.ts');
const page=source('src/app/features/dispatches/dispatch-bulk/dispatch-bulk-entry.component.html');
const component=source('src/app/features/dispatches/dispatch-bulk/dispatch-bulk-entry.component.ts');
const controller=backend('KLMN.Api/Controllers/DispatchesController.cs');
const handler=backend('KLMN.Application/Dispatches/Commands/CreateBulkDispatch/CreateBulkDispatchCommandHandler.cs');
const validator=backend('KLMN.Application/Dispatches/Commands/CreateBulkDispatch/CreateBulkDispatchCommandValidator.cs');
const result=backend('KLMN.Application/Dispatches/Responses/BulkDispatchRowResponse.cs');
const single=backend('KLMN.Application/Dispatches/Commands/CreateDispatch/CreateDispatchCommandHandler.cs');

assert.match(routes,/path: 'dispatches\/bulk'[\s\S]{0,300}Dispatches\.Create[\s\S]{0,270}DispatchBulkEntryComponent/);
assert.match(nav,/label: 'Toplu Sevk Girişi'[\s\S]{0,180}route: '\/app\/dispatches\/bulk'[\s\S]{0,110}Dispatches\.Create/);
assert.match(service,/bulkCreate\(items:BulkDispatchItem\[\]\)/);
assert.match(service,/http\.post<BulkDispatchResponse>\(this\.url\+'\/bulk',\{items\}\)/);
assert.match(page,/Sevk Toplu Giriş Sonuç/);
assert.match(page,/Sevkleri Kaydet/);
assert.match(page,/type="file" accept="\.json,\.jsonl,\.txt,\.xml"/);
assert.match(page,/json-preview/);
assert.match(page,/row\.identityMasked/);
assert.match(page,/row\.resultCode/);
assert.match(page,/row\.dispatchNo/);
assert.match(page,/\[paginator\]="true"/);
assert.match(component,/MAX_BULK_DISPATCH_FILE_BYTES/);
assert.match(component,/parseBulkDispatchFile\(raw\)/);
assert.match(component,/this\.submitted\.set\(true\)/);
// Süreç başarısız olsa da belirsiz sonucu otomatik yeniden göndermez.
assert.match(component,/Tekrar göndermeden önce Sevk Listesi/);
assert.match(controller,/\[HttpPost\("bulk"\)\][\s\S]{0,80}HasPermission\(PermissionCodes\.Dispatches\.Create\)/);
assert.match(controller,/RequestSizeLimit\(2\*1024\*1024\)/);
assert.match(controller,/sender\.Send\(command,ct\)/);
assert.match(validator,/items is \{ Count: >= 1 and <= 100 \}/);
assert.match(handler,/CreateAsyncScope\(\)/);
assert.match(handler,/new CreateDispatchCommand\(input\.Person,input\.Dispatch\)/);
assert.match(handler,/catch \(DispatchConflictException e\)/);
assert.match(handler,/Rows = rows/);
assert.match(result,/IdentityMasked/);
assert.doesNotMatch(result,/TcKimlikNo|YabanciKimlikNo/);
assert.match(single,/DispatchRules\.ValidateAsync/);
console.log('PASS: Toplu sevk route/menu, dosya yükleme, yetki, API, izolasyon ve sonuç sözleşmesi.');
