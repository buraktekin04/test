import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';

const root = new URL('../', import.meta.url);
const read = path => readFileSync(new URL(path, root), 'utf8');
const dispatchHtml = read('src/app/features/dispatches/dispatch-list/dispatch-list.component.html');
const dispatchTs = read('src/app/features/dispatches/dispatch-list/dispatch-list.component.ts');
const dispatchApi = read('src/app/features/dispatches/services/dispatch.service.ts');
const provisionHtml = read('src/app/features/provisions/query/provision-query.component.html');
const provisionTs = read('src/app/features/provisions/query/provision-query.component.ts');
const provisionModel = read('src/app/features/provisions/models/provision.model.ts');

assert.match(dispatchHtml, /header="Sevk Detay"/);
assert.match(dispatchHtml, /Kişi Bilgileri/);
assert.match(dispatchHtml, /Askerî Bilgileri/);
assert.match(dispatchHtml, /Sevk Bilgileri/);
assert.match(dispatchHtml, /detailPhotoUrl/);
assert.match(dispatchHtml, /Sevk İptali/);
assert.match(dispatchHtml, /İptal Açıklaması/);
assert.match(dispatchTs, /api\.cancel\(row,reason\)/);
assert.match(dispatchApi, /reason:string/);
assert.match(dispatchTs, /loadDetailNames/);
assert.match(dispatchHtml, /Görüntülenecek Kolonlar/);

assert.match(provisionHtml, /Bağlı Organizasyonları da Listele/);
assert.match(provisionTs, /includeRelatedOrganizations:/);
assert.match(provisionModel, /includeRelatedOrganizations\?:boolean/);
assert.match(provisionHtml, /header="Provizyon Detayı"/);
assert.match(provisionHtml, /Kişi Bilgileri/);
assert.match(provisionHtml, /Askerî Bilgileri/);
assert.match(provisionHtml, /Provizyon Bilgileri/);
assert.match(provisionHtml, /Görüntülenecek Kolonlar/);

console.log('Legacy dispatch/provision query UI regression: OK');
