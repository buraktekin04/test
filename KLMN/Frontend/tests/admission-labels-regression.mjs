import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = resolve(dirname(fileURLToPath(import.meta.url)), '../..');
const read = file => readFileSync(resolve(root, file), 'utf8');
const ui = read('Frontend/src/app/features/admission-types/admission-type-list/admission-type-list.component.ts');
const html = read('Frontend/src/app/features/admission-types/admission-type-list/admission-type-list.component.html');
const controller = read('Backend/KLMN.Api/Controllers/AdmissionTypesController.cs');
const entity = read('Backend/KLMN.Domain/Entities/Admissions/HcAdmissionTypeDef.cs');
const response = read('Backend/KLMN.Application/AdmissionTypes/Responses/HcAdmissionTypeDefResponse.cs');
const model = read('Frontend/src/app/features/admission-types/models/admission-types.model.ts');
const create = read('Backend/KLMN.Application/AdmissionTypes/Commands/CreateHcAdmissionTypeDef/CreateHcAdmissionTypeDefCommand.cs');
const update = read('Backend/KLMN.Application/AdmissionTypes/Commands/UpdateHcAdmissionTypeDef/UpdateHcAdmissionTypeDefCommand.cs');

const labels = [
  ['forcesCommandRequired', 'ForcesCommandRequired', 'Kuvvet Komutanlığı', 'Kuvvet Gerekir'],
  ['lastNominationDateRequired', 'LastNominationDateRequired', 'Son Adaylık Tarihi', 'Nasbı Gerekir'],
  ['dispatchDocumentDateRequired', 'DispatchDocumentDateRequired', 'Sevk Belgesi Tarihi', 'Sevk Evrak Tarihi Gerekir'],
  ['dispatchDocumentNoRequired', 'DispatchDocumentNoRequired', 'Sevk Belgesi Numarası', 'Sevk Evrak Sayısı Gerekir']
];

assert.match(html, /@for \(group of requirementGroups;/);
assert.match(html, /@for \(setting of group.fields;/);
for (const [key, property, oldLabel, label] of labels) {
  assert.ok(ui.includes("{ key: '" + key + "', label: '" + label + "' }"), 'Form etiketi: ' + key);
  assert.ok(!ui.includes("{ key: '" + key + "', label: '" + oldLabel + "' }"), 'Eski form etiketi: ' + key);
  assert.ok(controller.includes('new("' + label + '", x => x.' + property + ' ?'), 'Excel başlığı: ' + key);
  assert.ok(!controller.includes('new("' + oldLabel + '", x => x.' + property + ' ?'), 'Eski Excel başlığı: ' + key);
  assert.ok(model.includes(key + ': boolean;'), 'TypeScript API alanı değişti: ' + key);
  assert.ok(entity.includes('public bool ' + property + ' { get; set; }'), 'Entity alanı değişti: ' + property);
  assert.ok(response.includes('public bool ' + property + ' { get; init; }'), 'Response alanı değişti: ' + property);
  assert.ok(create.includes('bool ' + property + ','), 'Create komutu değişti: ' + property);
  assert.ok(update.includes('bool ' + property + ','), 'Update komutu değişti: ' + property);
}
assert.equal((ui.match(/\{ key: '[a-zA-Z]+Required', label: '/g) ?? []).length, 14,
  'Başvuru Grubu için 14 zorunluluk alanı korunmalı');
console.log('PASS: Form ve Excel yeni etiketleri eşleşiyor; 14 alan, API ve DB sözleşmesi korunuyor.');
