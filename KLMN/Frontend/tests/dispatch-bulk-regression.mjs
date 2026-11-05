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
console.log('PASS: Toplu sevk parser / eski ayrılmış JSON / kapsam ve kimlik kontrolü.');
