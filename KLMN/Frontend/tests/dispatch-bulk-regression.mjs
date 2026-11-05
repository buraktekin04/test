import assert from 'node:assert/strict';
import {parseBulkDispatchFile,maskBulkIdentity,MAX_BULK_DISPATCH_ROWS} from
 '../src/app/features/dispatches/dispatch-bulk/dispatch-bulk.parser.ts';

const idA='11111111-1111-4111-8111-111111111111';
const idB='22222222-2222-4222-8222-222222222222';
const idC='33333333-3333-4333-8333-333333333333';
const idD='44444444-4444-4444-8444-444444444444';
const person={tcKimlikNo:'12345678901',name:'Örnek',surname:'Kişi',birthDate:'1990-01-01'};
const dispatch={hcAdmissionTypeDefId:idA,reasonForExaminationDefId:idB,dispatchScope:2,
 dispatchScopeCityId:idC,senderMilitaryUnitDefId:idD};
const row={person,dispatch};
let parsed=parseBulkDispatchFile(JSON.stringify({items:[row]}));
assert.equal(parsed.items.length,1);
assert.equal(parsed.problems.length,0);
assert.equal(parsed.items[0].dispatch.dispatchScope,2);
assert.equal(parsed.items[0].dispatch.senderMilitaryUnitDefId,idD);

parsed=parseBulkDispatchFile(JSON.stringify({sevkYeniGiris:[row,row]}));
assert.equal(parsed.items.length,2);
assert.equal(parsed.items[1].person.name,'Örnek');
parsed=parseBulkDispatchFile(JSON.stringify(row)+','+JSON.stringify(row));
assert.equal(parsed.items.length,2);
parsed=parseBulkDispatchFile(JSON.stringify(row)+'\n'+JSON.stringify(row));
assert.equal(parsed.items.length,2);
parsed=parseBulkDispatchFile(JSON.stringify([row,{person,dispatch:{...dispatch,dispatchScope:1}}]));
assert.equal(parsed.items.length,1);
assert.equal(parsed.problems.length,1);
assert.equal(parsed.problems[0].rowNumber,2);
assert.match(parsed.problems[0].message,/kapsamı 2/);

assert.throws(()=>parseBulkDispatchFile('<root><sevk/></root>'),/gerçek XML/);
assert.throws(()=>parseBulkDispatchFile(''),/boş/);
assert.throws(()=>parseBulkDispatchFile(JSON.stringify(Array(MAX_BULK_DISPATCH_ROWS+1).fill(row))),/en fazla/);
assert.equal(maskBulkIdentity(person.tcKimlikNo),'*******8901');

const flat={tcKimlikNo:'12345678901',adi:'Örnek',soyadi:'Kişi',dogumTarihi:'1990-01-01',
 skBasvuruGrubu:idA,muayeneSebebi:idB,sevkKapsami:3,sevkKapsamiHastane:idC,
 senderChairChoice:'office:'+idD};
parsed=parseBulkDispatchFile(JSON.stringify([flat]));
assert.equal(parsed.items.length,1);
assert.equal(parsed.items[0].dispatch.senderMilitaryOfficeDefId,idD);
assert.equal(parsed.items[0].dispatch.dispatchScope,3);

// Eski MSKBS dosyasında dışarıdan gelen GUID'ler doğrudan korunur;
// yeni person/dispatch nesnelerini dosyada oluşturmak zorunlu değildir.
const oldSchema={
 UniqueRefno:12345678901,Name:'Örnek',Surname:'Kişi',BirthDate:'1990-01-01',
 HcAdmissionTypeDef:idA,ReasonForExamination:idB,
 DispatchScope:2,DispatchScopeProvince:idC,SenderChair:idD,
 ForcesCommand:'AA000000-0000-4000-8000-000000000001',
 Rank:'AA000000-0000-4000-8000-000000000002',
 MilitaryClass:'AA000000-0000-4000-8000-000000000003',
 Nationality:'AA000000-0000-4000-8000-000000000004',
 TownOfBirth:'AA000000-0000-4000-8000-000000000005',
 MilitaryUnit:'AA000000-0000-4000-8000-000000000006',
 MilitarySchool:'AA000000-0000-4000-8000-000000000007',
 MilitaryOffice:'AA000000-0000-4000-8000-000000000008',
 FlyingMission:'AA000000-0000-4000-8000-000000000009',
 FlyingPlaneType:'AA000000-0000-4000-8000-000000000010',
 LastNominationDate:'2026-01-30T10:00:00',BdPlace:'Ankara',BdCode:'06'
};
const oldFile=JSON.stringify(oldSchema);
parsed=parseBulkDispatchFile(oldFile);
assert.equal(parsed.problems.length,0);
assert.equal(parsed.items.length,1);
assert.equal(parsed.items[0].person.tcKimlikNo,'12345678901');
assert.equal(parsed.items[0].person.nationalityId,oldSchema.Nationality);
assert.equal(parsed.items[0].person.townOfBirthId,oldSchema.TownOfBirth);
assert.equal(parsed.items[0].dispatch.hcAdmissionTypeDefId,idA);
assert.equal(parsed.items[0].dispatch.reasonForExaminationDefId,idB);
assert.equal(parsed.items[0].dispatch.dispatchScopeCityId,idC);
assert.equal(parsed.items[0].legacySenderChairId,idD);
assert.equal(parsed.items[0].dispatch.senderMilitaryUnitDefId,null);
assert.equal(parsed.items[0].dispatch.senderMilitaryOfficeDefId,null);
assert.equal(parsed.items[0].dispatch.forcesCommandDefId,oldSchema.ForcesCommand);
assert.equal(parsed.items[0].dispatch.rankDefId,oldSchema.Rank);
assert.equal(parsed.items[0].dispatch.militaryClassDefId,oldSchema.MilitaryClass);
assert.equal(parsed.items[0].dispatch.militaryUnitDefId,oldSchema.MilitaryUnit);
assert.equal(parsed.items[0].dispatch.militarySchoolDefId,oldSchema.MilitarySchool);
assert.equal(parsed.items[0].dispatch.militaryOfficeDefId,oldSchema.MilitaryOffice);
assert.equal(parsed.items[0].dispatch.flyingMissionDefId,oldSchema.FlyingMission);
assert.equal(parsed.items[0].dispatch.planeTypeDefId,oldSchema.FlyingPlaneType);
assert.equal(parsed.items[0].dispatch.lastNominationDate,'2026-01-30');
assert.equal(parsed.items[0].person.birthPlaceText,'Ankara');

parsed=parseBulkDispatchFile(oldFile+','+oldFile);
assert.equal(parsed.items.length,2);
assert.equal(parsed.items[1].legacySenderChairId,idD);
parsed=parseBulkDispatchFile(JSON.stringify({SevkYeniGiris:[oldSchema]}));
assert.equal(parsed.items.length,1);
assert.equal(parsed.items[0].legacySenderChairId,idD);
parsed=parseBulkDispatchFile(JSON.stringify([oldSchema,{...oldSchema,Nationality:'9980'}]));
assert.equal(parsed.items.length,1);
assert.equal(parsed.problems.length,1);
assert.equal(parsed.problems[0].rowNumber,2);
assert.match(parsed.problems[0].message,/Uyruk.*GUID/);
parsed=parseBulkDispatchFile(JSON.stringify([{...oldSchema,SenderChair:idD,SenderMilitaryUnitDefId:idA}]));
assert.equal(parsed.items.length,0);
assert.match(parsed.problems[0].message,/tam olarak biri/);

// JSON uzantısı değil dosyanın içeriği belirleyici; .xml de eski MSKBS'de JSON idi.
console.log('PASS: Eski MSKBS alanları ve GUID değerleri birebir korunuyor; belirsiz ilişki reddedilir.');
