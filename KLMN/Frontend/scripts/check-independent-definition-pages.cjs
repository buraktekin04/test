// KLMN: her tanım ekranı ayrı component/HTML/SCSS/model/servis olmalı.
// Frontend dizininde: node scripts/check-independent-definition-pages.cjs
// Statik kaynak kontrolü; ng build ve uçtan uca testlerin yerine geçmez.
const fs=require('node:fs');
const path=require('node:path');
const assert=require('node:assert/strict');
const root=path.resolve(__dirname,'../src/app');
const definitions=[
 ['syy-diagnoses','syy-diagnosis-list','SyyDiagnosisListComponent','SYYDiagnoses','diagnosis'],
 ['syy-diagnosis-types','syy-diagnosis-type-list','SyyDiagnosisTypeListComponent','SYYDiagnosisTypes','lookup'],
 ['syy-diagnosis-slices','syy-diagnosis-slice-list','SyyDiagnosisSliceListComponent','SYYDiagnosisSlices','lookup'],
 ['annotation-reasons','annotation-reason-list','AnnotationReasonListComponent','AnnotationReasons','simple'],
 ['plane-types','plane-type-list','PlaneTypeListComponent','PlaneTypes','simple'],
 ['flying-missions','flying-mission-list','FlyingMissionListComponent','FlyingMissions','simple']
];
const routes=fs.readFileSync(path.join(root,'app.routes.ts'),'utf8');
const seen=new Set();
for(const [slug,name,klass,module,kind] of definitions){
 const feature=path.join(root,'features',slug);
 const prefix=path.join(feature,name,name+'.component.');
 for(const ext of ['ts','html','scss'])assert.ok(fs.existsSync(prefix+ext),slug+': '+ext+' dosyası yok');
 const ts=fs.readFileSync(prefix+'ts','utf8');
 const html=fs.readFileSync(prefix+'html','utf8');
 assert.ok(ts.includes('export class '+klass),slug+': bağımsız component eksik');
 assert.ok(ts.includes("templateUrl:'./"+name+".component.html'"),slug+': HTML bağımsız değil');
 assert.ok(routes.includes("path: '"+slug+"'"),slug+': route eksik');
 assert.ok(routes.includes('.then(x=>x.'+klass+')'),slug+': route yanlış component açıyor');
 assert.ok(routes.includes("permissionModule: '"+module+"'"),slug+': permission eksik');
 assert.ok(!ts.includes('SyyDefinitionListComponent'),slug+': generic sayfa yasak');
 const selector=ts.match(/selector:'([^']+)'/);
 assert.ok(selector&&!seen.has(selector[1]),slug+': selector benzersiz değil');
 seen.add(selector[1]);
 for(const layer of ['services','models']){
  const re=new RegExp("from '\\.\\./"+layer+"/([^']+)'");
  const imp=ts.match(re);
  assert.ok(imp,slug+': bağımsız '+layer+' import eksik');
  assert.ok(fs.existsSync(path.join(feature,layer,imp[1]+'.ts')),slug+': bağımsız '+layer+' dosyası yok');
 }
 assert.ok(html.includes('<p-table')&&html.includes('pResizableColumn'),slug+': tablo standardı');
 for(const field of ['syyDiagnosisType','attachedRecNo','slice','eligible']){
  const expected=kind==='diagnosis';
  assert.equal(html.includes('formControlName="'+field+'"'),expected,slug+': yanlış alan '+field);
 }
 for(const field of ['validFrom','validTo'])
  assert.equal(html.includes('formControlName="'+field+'"'),kind!=='lookup',slug+': yanlış tarih alanı '+field);
}
assert.ok(!fs.existsSync(path.join(root,'features/syy-definitions/definition-list/definition-list.component.ts')),
  'Eski generic component dosyası kaldırılmalı');
console.log('PASS: 6 tanım ekranında ayrı component, şablon, model, servis, route ve alanlar doğrulandı.');
