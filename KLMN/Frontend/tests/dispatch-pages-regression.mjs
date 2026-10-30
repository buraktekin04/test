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
const entryScss=read('src/app/features/dispatches/dispatch-entry/dispatch-entry.component.scss');
const appConfig=read('src/app/app.config.ts');
const genderFilter=read('src/app/core/filters/case-insensitive-filter.ts');
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
assert.match(entryHtml,/class="dispatch-form compact-dispatch-form"/);
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
assert.match(entryHtml,/Önce başvuru grubu/);
assert.match(entryHtml,/class="entry-workspace"/);
assert.match(entryHtml,/class="required-mark"/);
assert.match(entryHtml,/rulesLoading\(\)/);
assert.match(entryTs,/readonly militarySelections=/);
assert.match(entryHtml,/entry-military-grid/);

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
assert.match(entryHtml,/formControlName="militaryUnitDescription" rows="2"/);
assert.match(entryHtml,/formControlName="senderChairOther" rows="2"/);
assert.doesNotMatch(entryHtml,/formControlName="description"/);
assert.doesNotMatch(entryHtml,/>Açıklama\s*</);

// Eski MSKBS'ye yakın kompakt yerleşim: kimlik üstte, kişi + sevk solda,
 // askerî/başvuru sağda; tüm seçimler doğrudan görünür.
assert.match(entryHtml,/class="entry-lookup-bar"/);
assert.match(entryHtml,/class="entry-left-column"/);
assert.match(entryHtml,/class="form-section compact-section entry-right-column"/);
assert.match(entryHtml,/class="entry-footer"/);
assert.match(entryHtml,/class="form-grid entry-person-grid"/);
assert.match(entryHtml,/class="form-grid entry-dispatch-grid"/);
assert.match(entryHtml,/class="form-grid entry-military-grid"/);
assert.match(entryHtml,/label="Nüfustan Sorgula"/);
assert.match(entryHtml,/label="QR Kod"/);
assert.match(entryHtml,/app-dispatch-photo-capture/);
assert.match(entryHtml,/formControlName="hcAdmissionTypeDefId"/);
assert.match(entryHtml,/formControlName="reasonForExaminationDefId"/);
assert.doesNotMatch(entryHtml,/<details|entry-field-group|entry-priority-panel/);
assert.match(entryScss,/\.entry-workspace\{display:grid;grid-template-columns:/);
assert.match(entryScss,/position:sticky;bottom:0/);
assert.match(entryScss,/@media\(max-width:920px\)/);
assert.match(entryScss,/@media\(max-width:650px\)/);
assert.match(entryTs,/styleUrl:'\.\/dispatch-entry\.component\.scss'/);
assert.doesNotMatch(entryTs,/dispatch-list\.component\.scss/);

// Sevk listesi filtre seçilmeden ve Sorgula'ya basılmadan sunucuya gitmez.
assert.match(listTs,/readonly hasSearched=signal\(false\)/);
assert.match(listTs,/private appliedForm:/);
assert.doesNotMatch(listTs,/ngOnInit\(\)\s*\{[\s\S]*?if\(this\.canView\(\)\)this\.load\(\)/);
assert.match(listHtml,/@if\(hasSearched\(\)\)\{/);
assert.match(listHtml,/Sevk sorgusu henüz çalıştırılmadı/);
assert.match(listTs,/this\.appliedForm=\{\.\.\.values\}/);
assert.match(listTs,/this\.hasSearched\.set\(true\)/);
assert.match(listTs,/this\.hasSearched\.set\(false\)/);

// Dropdown filtrasyonu hem global PrimeNG mekanizması hem sunucu araması için tutarlı.
assert.match(appConfig,/filters\.register\('contains',containsIgnoringCase\)/);
assert.match(appConfig,/filters\.register\('startsWith',startsWithIgnoringCase\)/);
assert.match(appConfig,/filters\.register\('endsWith',endsWithIgnoringCase\)/);
assert.match(genderFilter,/toLocaleLowerCase\('tr-TR'\)/);

// Cinsiyet enum sabitleri yok, API sözlüğü ve eski 1/2/3 kodları korunur.
assert.match(entryTs,/this\.api\.genders\(\)/);
assert.match(entryTs,/genderItems=computed\(/);
assert.doesNotMatch(entryTs,/readonly genderItems=\[/);
assert.match(entryHtml,/\[options\]="genderItems\(\)"/);
assert.match(service,/genders\(\)/);

// Yalnız Sevk Giriş ekranından belge ekleme kaldırıldı; fotoğraf akışı kalır.
assert.doesNotMatch(entryHtml,/Belge Ekle|entry-file-picker|selectFile\(\$event\)|uploadFile\(\)/);
assert.doesNotMatch(entryTs,/selectFile\(|this\.uploadFile\(|this\.files\(|this\.loadFiles\(/);
assert.match(entryHtml,/app-dispatch-photo-capture/);
assert.match(entryTs,/this\.api\.uploadPhoto/);

console.log('PASS: Sevk sorgusu yalnız Sorgula, global case-insensitive filtre, DB cinsiyet ve fotoğraf.');
