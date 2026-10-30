import {readFileSync} from 'node:fs';
import assert from 'node:assert/strict';
import {fileURLToPath} from 'node:url';
import {dirname,resolve} from 'node:path';

const root=resolve(dirname(fileURLToPath(import.meta.url)),'..');
const read=p=>readFileSync(resolve(root,p),'utf8');

const routes=read('src/app/app.routes.ts');
const navigation=read('src/app/layout/services/navigation.service.ts');
const listTs=read('src/app/features/dispatches/dispatch-list/dispatch-list.component.ts');
const listHtml=read('src/app/features/dispatches/dispatch-list/dispatch-list.component.html');
const entryTs=read('src/app/features/dispatches/dispatch-entry/dispatch-entry.component.ts');
const entryHtml=read('src/app/features/dispatches/dispatch-entry/dispatch-entry.component.html');
const service=read('src/app/features/dispatches/services/dispatch.service.ts');
const sidebarHtml=read('src/app/layout/sidebar/sidebar.component.html');
const listScss=read('src/app/features/dispatches/dispatch-list/dispatch-list.component.scss');

assert.match(routes,/path: 'dispatches'[\s\S]{0,390}DispatchListComponent/);
assert.match(routes,/path: 'dispatches\/new'[\s\S]{0,400}DispatchEntryComponent/);
assert.match(routes,/path: 'dispatches\/:id\/edit'[\s\S]{0,400}DispatchEntryComponent/);
assert.match(routes,/permissions:\[PermissionCodes\.Dispatches\.Create\]/);
assert.match(routes,/permissions:\[PermissionCodes\.Dispatches\.Update\]/);

assert.match(navigation,/label: 'Sevk Girişi'[\s\S]{0,200}route: '\/app\/dispatches\/new'/);
assert.match(navigation,/label: 'Sevk Listesi ve Sorgulama'[\s\S]{0,200}route: '\/app\/dispatches'/);

assert.match(listHtml,/formControlName="currentState"/);
assert.match(listHtml,/formControlName="dateFrom"/);
assert.match(listHtml,/formControlName="dateTo"/);
for(const key of ['dispatchNo','purchaseId','tcKimlikNo','yabanciKimlikNo',
 'name','surname','senderAuthority','includeRelatedOrganizations','dispatchScope',
 'dispatchScopeCityId','dispatchScopeHospitalId']){
 assert.match(listHtml,new RegExp('formControlName="'+key+'"'));
}
assert.match(listHtml,/Ayrıntılı Filtreler/);
assert.match(listHtml,/p-table/);
assert.doesNotMatch(listHtml,/dispatch-form|app-dispatch-qr-reader|app-dispatch-photo-capture/);
assert.match(entryHtml,/class="dispatch-form"/);
assert.match(entryHtml,/app-dispatch-qr-reader/);
assert.match(entryHtml,/app-dispatch-photo-capture/);
assert.doesNotMatch(entryHtml,/p-table|formControlName="search"/);
assert.match(entryTs,/this\.router\.navigate\(\['\/app\/dispatches'/);
assert.match(entryTs,/this\.route\.snapshot\.paramMap\.get\('id'\)/);
assert.match(listTs,/this\.router\.navigate\(\['\/app\/dispatches',row\.id,'edit'\]\)/);
assert.match(service,/documentDateFrom/);
assert.match(service,/statuses\(\)/);
assert.match(listTs,/readonly statusItems=computed\(/);
assert.doesNotMatch(listTs,/readonly statusItems=\[/);
assert.match(listTs,/this\.api\.statuses\(\)/);
assert.match(listTs,/senderMilitaryUnitDefId/);
assert.match(listTs,/includeRelatedOrganizations/);
assert.match(entryHtml,/Başvuru grubu seçilmeden muayene sebebi seçilemez/);
assert.match(entryHtml,/class="entry-field-group"/);
assert.match(entryHtml,/class="required-mark"/);
assert.match(entryHtml,/rulesLoading\(\)/);
assert.match(entryTs,/readonly militarySelections=/);
assert.match(entryTs,/hasMandatoryMilitary\(\)/);

// /app/dispatches/new iken /app/dispatches liste menüsü ayrıca aktif olmamalı.
assert.match(sidebarHtml,/routerLinkActiveOptions[^\n]*item\.route === '\/app\/dispatches'/);
assert.match(sidebarHtml,/routerLinkActive="active"/);
// Flex-basis 250px yalnız üst seviye arama input'unda olmalı; tarih kutularının
// yüksekliği sabit tutulmalı (iç içe flex-column içinde flex-grow:1 uygulanmaz).
assert.match(listScss,/\.filter-panel form > input\{flex:1 1 250px/);
assert.doesNotMatch(listScss,/\.filter-panel input\{flex:1 1 250px/);
assert.match(listScss,/\.filter-field > input\[type="date"\]\s*\{[\s\S]*?flex:none;/);
assert.match(listScss,/\.filter-field > input\[type="date"\]\s*\{[\s\S]*?height:38px;/);
assert.match(listScss,/\.filter-grid\{display:grid;/);

// Kişi sorgusu artık yerel Persons veritabanını değil, nüfus entegrasyonu endpoint'ini çağırır.
assert.match(entryHtml,/label="Nüfustan Sorgula"/);
assert.doesNotMatch(entryHtml,/Yerel Kaydı Sorgula|MERNİS entegrasyonu değildir|Yerel Kaydı/);
assert.match(entryTs,/this\.api\.queryPopulation\(/);
assert.match(service,/persons\/national-registry/);
assert.doesNotMatch(service,/persons\/lookup/);
assert.match(entryHtml,/formControlName="militaryUnitDescription" rows="3"/);
assert.match(entryHtml,/formControlName="senderChairOther" rows="3"/);
assert.doesNotMatch(entryHtml,/formControlName="description"/);
assert.doesNotMatch(entryHtml,/>Açıklama\s*</);

console.log('PASS: Sevk filtreleri, nüfus sorgu bağlantısı ve açıklama alanları.');
