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
assert.match(listHtml,/formControlName="documentDateFrom"/);
assert.match(listHtml,/formControlName="documentDateTo"/);
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

// /app/dispatches/new iken /app/dispatches liste menüsü ayrıca aktif olmamalı.
assert.match(sidebarHtml,/routerLinkActiveOptions[^\n]*item\.route === '\/app\/dispatches'/);
assert.match(sidebarHtml,/routerLinkActive="active"/);
// Flex-basis 250px yalnız üst seviye arama input'unda olmalı; tarih kutularının
// yüksekliği sabit tutulmalı (iç içe flex-column içinde flex-grow:1 uygulanmaz).
assert.match(listScss,/\.filter-panel form > input\{flex:1 1 250px/);
assert.doesNotMatch(listScss,/\.filter-panel input\{flex:1 1 250px/);
assert.match(listScss,/\.filter-field > input\[type="date"\]\s*\{[\s\S]*?flex:none;/);
assert.match(listScss,/\.filter-field > input\[type="date"\]\s*\{[\s\S]*?height:38px;/);

console.log('PASS: Sevk Girişi / Sevk Listesi rotaları, seçili menü, tarih input CSS ve diğer Sevk regresyonları.');
