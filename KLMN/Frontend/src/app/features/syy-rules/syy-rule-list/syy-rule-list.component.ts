import {Component,computed,inject,OnInit,signal} from '@angular/core';
import {HttpErrorResponse} from '@angular/common/http';
import {DatePipe} from '@angular/common';
import {ActivatedRoute,RouterLink} from '@angular/router';
import {ReactiveFormsModule,FormBuilder} from '@angular/forms';
import {finalize} from 'rxjs';
import {MessageService} from 'primeng/api';
import {ButtonModule} from 'primeng/button';
import {CheckboxModule} from 'primeng/checkbox';
import {InputTextModule} from 'primeng/inputtext';
import {TableModule} from 'primeng/table';
import {TagModule} from 'primeng/tag';
import {ToastModule} from 'primeng/toast';
import {PermissionService} from '../../../core/permissions/services/permission.service';
import {SyyRule,SyyRuleQuery} from '../models/syy-rule.model';
import {SyyRuleService} from '../services/syy-rule.service';
/** SYY Kural Tanımları: Eligible=true filtreli salt okunur tanı ve eşleştirme ekranı. */
@Component({
 selector:'app-syy-rule-list',standalone:true,
 imports:[DatePipe,RouterLink,ReactiveFormsModule,ButtonModule,CheckboxModule,InputTextModule,TableModule,TagModule,ToastModule],
 providers:[MessageService],
 templateUrl:'./syy-rule-list.component.html',
 styleUrl:'./syy-rule-list.component.scss'
})
export class SyyRuleListComponent implements OnInit {
 private readonly api=inject(SyyRuleService);
 private readonly fb=inject(FormBuilder);
 private readonly route=inject(ActivatedRoute);
 private readonly permissions=inject(PermissionService);
 private readonly messages=inject(MessageService);
 readonly items=signal<SyyRule[]>([]);
 readonly loading=signal(false);
 readonly total=signal(0);
 readonly page=signal(1);
 readonly pageSize=signal(20);
 readonly presetId=this.route.snapshot.queryParamMap.get('syyDiagnosisId');
 readonly focusedId=signal<string|null>(this.presetId);
 readonly expandedId=signal<string|null>(this.presetId);
 readonly maxPage=computed(()=>Math.max(1,Math.ceil(this.total()/this.pageSize())));
 readonly canSeeBranches=computed(()=>this.permissions.hasModule('SYYDiagnosisBranches'));
 readonly canSeeProcedures=computed(()=>this.permissions.hasModule('SYYDiagnosisProcedures'));
 readonly canSeeApprovals=computed(()=>this.permissions.hasModule('SYYDiagnosisRequireds'));
 readonly form=this.fb.nonNullable.group({search:'',includeInactive:false});
 private requestId=0;
 ngOnInit():void{this.load();}
 private query():SyyRuleQuery{
  const values=this.form.getRawValue();
  return {search:values.search,includeInactive:values.includeInactive,
    pageNumber:this.page(),pageSize:this.pageSize(),syyDiagnosisDefId:this.focusedId()};
 }
 load():void{
  const id=++this.requestId;this.loading.set(true);
  this.api.list(this.query()).pipe(finalize(()=>{if(id===this.requestId)this.loading.set(false);}))
   .subscribe({
    next:result=>{
     if(id!==this.requestId)return;
     this.items.set(result.items);this.total.set(result.totalCount);this.expandedId.set(this.focusedId());
    },
    error:error=>{
     if(id!==this.requestId)return;
     this.items.set([]);this.total.set(0);this.notice(error);
    }
   });
 }
 search():void{this.page.set(1);this.load();}
 clear():void{this.form.reset({search:'',includeInactive:false});this.focusedId.set(null);this.search();}
 toPage(value:number):void{
  if(value<1||value>this.maxPage()||this.loading())return;
  this.page.set(value);this.load();
 }
 changePageSize(e:Event):void{
  const size=Number((e.target as HTMLSelectElement).value);
  if(![10,20,50,100].includes(size))return;
  this.pageSize.set(size);this.search();
 }
 toggle(id:string):void{this.expandedId.set(this.expandedId()===id?null:id);}
 private notice(error:unknown):void{
  const e=error as HttpErrorResponse;
  this.messages.add({severity:'error',summary:'Kural listesi alınamadı',
   detail:e.error?.detail||e.error?.title||e.message||'İşlem gerçekleştirilemedi.',life:6500});
 }
}
