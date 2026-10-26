import {Component,Input,Output,EventEmitter,OnChanges,computed,inject,signal} from '@angular/core';
import {finalize} from 'rxjs';
import {ButtonModule} from 'primeng/button';
import {DialogModule} from 'primeng/dialog';
import {UserListItem} from '../models/user-list-item.model';
import {UserPermissionSettings,UserPermissionSetting} from '../models/user-permission-settings.model';
import {UserService} from '../services/user.service';

type OverrideChoice='inherit'|'grant'|'deny';
type DisplayRow=UserPermissionSetting&{selected:OverrideChoice;original:OverrideChoice};
interface ModuleGroup {module:string;rows:DisplayRow[];}

/** Role ait hakları bozmadan kullanıcıya özel ek izin veya açık ret verebilen yönetim paneli. */
@Component({
 selector:'app-user-permission-dialog',standalone:true,
 imports:[ButtonModule,DialogModule],
 templateUrl:'./user-permission-dialog.component.html',
 styleUrl:'./user-permission-dialog.component.scss'
})
export class UserPermissionDialogComponent implements OnChanges {
 @Input() user:UserListItem|null=null;
 @Output() closed=new EventEmitter<void>();
 @Output() saved=new EventEmitter<void>();
 private readonly service=inject(UserService);
 readonly snapshot=signal<UserPermissionSettings|null>(null);
 readonly rows=signal<DisplayRow[]>([]);
 readonly filter=signal('');
 readonly loading=signal(false);
 readonly saving=signal(false);
 readonly error=signal<string|null>(null);
 private requestId=0;

 readonly changed=computed(()=>this.rows().some(row=>row.selected!==row.original));
 readonly groups=computed<ModuleGroup[]>(()=>{
    const query=this.filter().trim().toLocaleLowerCase('tr-TR');
    const rows=this.rows().filter(row=>!query ||
      [row.module,row.name,row.code].some(x=>x.toLocaleLowerCase('tr-TR').includes(query)));
    const groups=new Map<string,DisplayRow[]>();
    for(const row of rows)groups.set(row.module,[...(groups.get(row.module)??[]),row]);
    return [...groups].map(([module,items])=>({module,rows:items}));
 });
 public ngOnChanges():void {
   this.requestId++;
   this.error.set(null);this.filter.set('');this.rows.set([]);this.snapshot.set(null);
   if(this.user)this.load();
 }
 public load():void {
   const user=this.user;if(!user)return;
   const id=++this.requestId;this.loading.set(true);this.error.set(null);
   this.service.getUserPermissionSettings(user.id).pipe(
     finalize(()=>{if(id===this.requestId)this.loading.set(false);})
   ).subscribe({
     next:r=>{
       if(id!==this.requestId)return;
       this.snapshot.set(r);
       this.rows.set(r.permissions.map(p=>{
         const selected:OverrideChoice=p.isGrantedOverride===null?'inherit':p.isGrantedOverride?'grant':'deny';
         return {...p,selected,original:selected};
       }));
     },
     error:e=>{if(id!==this.requestId)return;this.error.set(e.error?.detail||'Yetkiler yüklenemedi.');}
   });
 }
 public setChoice(id:string,value:string):void {
   if(value!=='inherit'&&value!=='grant'&&value!=='deny')return;
   this.rows.update(items=>items.map(x=>x.permissionId===id?{...x,selected:value}:x));
 }
 public effective(row:DisplayRow):boolean {
   return this.snapshot()?.isAdmin === true || row.selected==='grant'||(row.selected==='inherit'&&row.grantedByRole);
 }
 public resetOverrides():void {
   this.rows.update(items=>items.map(x=>({...x,selected:'inherit'})));
 }
 public close():void{if(!this.saving())this.closed.emit();}
 public save():void {
   const user=this.user,settings=this.snapshot();
   if(!user||!settings||this.loading()||this.saving()||settings.isAdmin)return;
   if(!this.changed()){this.closed.emit();return;}
   this.saving.set(true);this.error.set(null);
   this.service.updateUserPermissionSettings({
     userId:settings.userId,version:settings.version,
     choices:this.rows().map(row=>({permissionId:row.permissionId,
       isGranted:row.selected==='inherit'?null:row.selected==='grant'}))
   }).pipe(finalize(()=>this.saving.set(false))).subscribe({
     next:()=>this.saved.emit(),
     error:e=>this.error.set(e.error?.detail||'Özel yetkiler kaydedilemedi. Listeyi yenileyiniz.')
   });
 }
}
