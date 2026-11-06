import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import {dirname,resolve} from 'node:path';
import {fileURLToPath} from 'node:url';

const root=resolve(dirname(fileURLToPath(import.meta.url)),'..');
const read=p=>readFileSync(resolve(root,p),'utf8');
const backend=p=>read('../Backend/'+p);
const routes=read('src/app/app.routes.ts');
const menu=read('src/app/layout/services/navigation.service.ts');
const permissions=read('src/app/core/permissions/constants/permission-codes.ts');
const service=read('src/app/features/provisions/services/provision.service.ts');
const dispatchList=read('src/app/features/dispatches/dispatch-list/dispatch-list.component.ts');
const dispatchHtml=read('src/app/features/dispatches/dispatch-list/dispatch-list.component.html');
const withTs=read('src/app/features/provisions/with-dispatch/provision-with-dispatch.component.ts');
const withHtml=read('src/app/features/provisions/with-dispatch/provision-with-dispatch.component.html');
const withoutTs=read('src/app/features/provisions/without-dispatch/provision-without-dispatch.component.ts');
const withoutHtml=read('src/app/features/provisions/without-dispatch/provision-without-dispatch.component.html');
const queryTs=read('src/app/features/provisions/query/provision-query.component.ts');
const queryHtml=read('src/app/features/provisions/query/provision-query.component.html');
const trackingTs=read('src/app/features/provisions/tracking/provision-tracking.component.ts');
const trackingHtml=read('src/app/features/provisions/tracking/provision-tracking.component.html');
const db=backend('KLMN.Persistence/Contexts/KLMNDbContext.cs');
const iface=backend('KLMN.Application/Common/Interfaces/Persistence/IKLMNDbContext.cs');
const entity=backend('KLMN.Domain/Entities/Admissions/Provision.cs');
const state=backend('KLMN.Domain/Entities/Admissions/ProvisionState.cs');
const config=backend('KLMN.Persistence/Configurations/Admissions/ProvisionConfiguration.cs');
const stateConfig=backend('KLMN.Persistence/Configurations/Admissions/ProvisionStateConfiguration.cs');
const statusEntity=backend('KLMN.Domain/Entities/Admissions/ProvisionStatusDef.cs');
const statusCodes=backend('KLMN.Domain/Entities/Admissions/ProvisionStatusCodes.cs');
const statusConfig=backend('KLMN.Persistence/Configurations/Admissions/ProvisionStatusDefConfiguration.cs');
const statusController=backend('KLMN.Api/Controllers/ProvisionsController.cs');
const statusHandler=backend('KLMN.Application/Provisions/Queries/GetProvisionStatuses/GetProvisionStatusesQueryHandler.cs');
const styles=read('src/app/features/provisions/provision-common.scss');
const ctrl=backend('KLMN.Api/Controllers/ProvisionsController.cs');
const withHandler=backend('KLMN.Application/Provisions/Commands/CreateWithDispatch/CreateWithDispatchCommandHandler.cs');
const withoutHandler=backend('KLMN.Application/Provisions/Commands/CreateWithoutDispatch/CreateWithoutDispatchCommandHandler.cs');
const cancelHandler=backend('KLMN.Application/Provisions/Commands/CancelProvision/CancelProvisionCommandHandler.cs');
const listHandler=backend('KLMN.Application/Provisions/Queries/GetProvisions/GetProvisionsQueryHandler.cs');
const historyHandler=backend('KLMN.Application/Provisions/Queries/GetProvisionStates/GetProvisionStatesQueryHandler.cs');
const seeded=backend('KLMN.Persistence/Seeds/IdentitySeedData.cs');

for(const x of ['with-dispatch','without-dispatch','query','tracking']){
 assert.match(routes,new RegExp("path: 'provisions/"+x+"'"));
 assert.ok(menu.includes("/app/provisions/"+x));
}
assert.match(routes,/PermissionCodes\.Provisions\.CreateWithDispatch/);
assert.match(routes,/PermissionCodes\.Provisions\.CreateWithoutDispatch/);
for(const x of ['View','Query','CreateWithDispatch','CreateWithoutDispatch','Cancel','Track']){
 assert.ok(permissions.includes("Provisions."+x));
 assert.ok(seeded.includes("PermissionCodes.Provisions."+x));
}
assert.match(dispatchList,/openProvision\(row:DispatchItem\)/);
assert.match(dispatchHtml,/Sevke provizyon al/);
assert.match(withHtml,/Sevk Kayıt Numarası/);
assert.match(withHtml,/Kabul Yapan Hastane/);
assert.match(withTs,/withDispatch\(v\.dispatchReference,v\.hospitalId\)/);
assert.match(withoutHtml,/Başvuran Kişi/);
assert.match(withoutHtml,/Provizyon Bilgileri/);
assert.match(withoutHtml,/Askerî Bilgiler/);
assert.match(withoutTs,/queryPopulation\(/);
assert.match(withoutTs,/admissionRules\(/);
assert.match(withoutTs,/type==='senders'/);
assert.match(withoutTs,/withoutDispatch\(v\.hospitalId,person,dispatch,null,photoBase64\)/);
assert.match(queryHtml,/Provizyon İptal/);
assert.match(queryTs,/canCancel=computed\(/);
assert.match(trackingHtml,/Provizyon Durumu Detay/);
assert.match(trackingHtml,/Provizyon Durumu Özet/);
assert.doesNotMatch(trackingHtml,/Eski sistem|Eski MSKBS| GUID/);
assert.match(trackingTs,/api\.states\(/);
for(const name of ['withDispatch','withoutDispatch','list','detail','states','cancel']){
 assert.ok(service.includes(name+'('));
}
assert.match(db,/DbSet<Provision> Provisions/);
assert.match(db,/DbSet<ProvisionState> ProvisionStates/);
assert.match(iface,/DbSet<Provision> Provisions/);
assert.match(entity,/public Guid DispatchId/);
assert.match(entity,/public Guid DispatchHospitalId/);
assert.match(entity,/public long ProvisionNo/);
assert.match(entity,/public Guid\? ObjectionableReportId/);
assert.match(state,/public Guid ProvisionId/);
assert.match(state,/public int State/);
assert.match(config,/ToTable\("Provisions"/);
assert.match(stateConfig,/ToTable\("ProvisionStates"/);
assert.match(ctrl,/HttpPost\("with-dispatch"\)/);
assert.match(ctrl,/HttpPost\("without-dispatch"\)/);
assert.match(ctrl,/HttpPatch\("\{id:guid\}\/cancel"\)/);
assert.match(withHandler,/x=>x\.Id==request\.HospitalId/);
assert.match(withHandler,/dispatch\.CurrentState=2/);
assert.match(withoutHandler,/DispatchRules\.ValidateAsync/);
assert.match(withoutHandler,/WithoutDispatch=1/);
assert.match(withoutHandler,/Provision=provision,State=ProvisionStatusCodes\.New/);
assert.match(withoutHandler,/photo=Convert\.FromBase64String/);
assert.match(cancelHandler,/provision\.CurrentState=ProvisionStatusCodes\.Cancelled/);
assert.match(cancelHandler,/provision\.Version!=request\.Version/);
assert.match(listHandler,/Skip\(\(request\.PageNumber-1\)\*request\.PageSize\)/);
assert.match(historyHandler,/ProvisionStates/);
// Durum adları tek bir merkezi tanım tablosundan okunur; UI'da magic-number eşlemesi yoktur.
assert.match(db,/DbSet<ProvisionStatusDef> ProvisionStatusDefs/);
assert.match(iface,/DbSet<ProvisionStatusDef> ProvisionStatusDefs/);
assert.match(statusEntity,/public int Code/);
assert.match(statusConfig,/ToTable\("ProvisionStatusDefs"\)/);
assert.match(statusConfig,/HasData\(/);
for(const name of ['New','Report','Cancelled','Notification']){
 assert.match(statusCodes,new RegExp('const int '+name+' ='));
 assert.match(statusConfig,new RegExp('ProvisionStatusCodes\\.'+name));
}
assert.match(config,/HasOne<ProvisionStatusDef>\(\)/);
assert.match(stateConfig,/HasOne<ProvisionStatusDef>\(\)/);
assert.match(statusController,/HttpGet\("statuses"\)/);
assert.match(statusHandler,/ProvisionStatusDefs\.AsNoTracking\(\)/);
assert.match(service,/statuses\(\)\{/);
assert.match(queryTs,/api\.statuses\(\)/);
assert.match(trackingTs,/api\.statuses\(\)/);
assert.doesNotMatch(queryTs,/\(\{1:'Yeni'/);
assert.doesNotMatch(trackingTs,/\(\{1:'Yeni'/);
assert.doesNotMatch(withHtml,/<header class="provision-header">/);
for(const html of [withHtml,withoutHtml,queryHtml,trackingHtml]){
 assert.doesNotMatch(html,/Eski MSKBS|Eski sistem|eski GUID| GUID:/i);
}
assert.match(withHtml,/Sevk numarasını giriniz/);
assert.match(withHtml,/class="provision-link-button"/);
assert.match(withoutHtml,/provision-link-button/);
assert.match(queryHtml,/provision-header-actions/);
assert.match(trackingHtml,/provision-link-button/);
assert.match(styles,/\.provision-field-label\{/);
assert.match(styles,/\.provision-link-button\{/);
for(const html of [withHtml,withoutHtml,queryHtml]){
 assert.doesNotMatch(html,/<label class="provision-field(?: wide)?">[^<\n]+ <span class="provision-required">\*<\/span>/);
}
console.log('PASS: Provizyon durumları referans tablosundan geliyor; metin, yıldız ve buton düzeni güncel.');
