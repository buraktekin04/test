import {inject,Injectable} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {API_BASE_URL} from '../../../core/config/api.tokens';
import {DispatchPerson,DispatchDetails,DispatchResponse,DispatchSaveResponse,DispatchItem,
 DispatchOption,OptionType,DispatchFileItem,DispatchPage,DispatchAdmissionRules} from '../models/dispatch.model';

/** Sevk, yerel kişi eşleştirme ve belge API istemcisi; doğrudan nüfus sistemi çağrısı içermez. */
@Injectable({providedIn:'root'})
export class DispatchService {
 private readonly http=inject(HttpClient);
 private readonly url=inject(API_BASE_URL)+'/dispatches';
 list(pageNumber:number,pageSize:number,search:string,includeCancelled:boolean,
  currentState:number|null=null,documentDateFrom='',documentDateTo=''){
  let params=new HttpParams().set('pageNumber',pageNumber).set('pageSize',pageSize)
    .set('includeCancelled',includeCancelled);
  if(search.trim())params=params.set('search',search.trim());
  if(currentState!==null)params=params.set('currentState',currentState);
  if(documentDateFrom)params=params.set('documentDateFrom',documentDateFrom);
  if(documentDateTo)params=params.set('documentDateTo',documentDateTo);
  return this.http.get<DispatchPage>(this.url,{params});
 }
 detail(id:string){return this.http.get<DispatchResponse>(this.url+'/'+id);}
 admissionRules(admissionTypeId:string){
  return this.http.get<DispatchAdmissionRules>(this.url+'/admission-rules/'+admissionTypeId);
 }
 lookupPerson(person:Pick<DispatchPerson,'tcKimlikNo'|'yabanciKimlikNo'|'birthDate'>){
  return this.http.post<{person:DispatchPerson}>(this.url+'/persons/lookup',person);
 }
 options(type:OptionType,search='',selectedId?:string){
  let params=new HttpParams().set('type',type).set('search',search);
  if(selectedId)params=params.set('selectedId',selectedId);
  return this.http.get<DispatchOption[]>(this.url+'/options',{params});
 }
 create(person:DispatchPerson,dispatch:DispatchDetails){
  return this.http.post<DispatchSaveResponse>(this.url,{person,dispatch});
 }
 update(id:string,version:number,person:DispatchPerson,dispatch:DispatchDetails){
  return this.http.put<DispatchSaveResponse>(this.url+'/'+id,{id,version,person,dispatch});
 }
 cancel(item:Pick<DispatchItem,'id'|'version'>){
  return this.http.patch<DispatchSaveResponse>(this.url+'/'+item.id+'/cancel',{id:item.id,version:item.version});
 }
 /** Fotoğraf için ayrı endpoint; kimlik görseli belge olarak karıştırılmaz. */
 getPhoto(id:string){return this.http.get(this.url+'/'+id+'/photo',{responseType:'blob'});}
 uploadPhoto(id:string,file:File){
  const body=new FormData();body.append('file',file,file.name);
  return this.http.post<DispatchSaveResponse>(this.url+'/'+id+'/photo',body);
 }
 deletePhoto(id:string,version:number){
  return this.http.delete<DispatchSaveResponse>(this.url+'/'+id+'/photo',
   {params:new HttpParams().set('version',version)});
 }
 files(id:string){return this.http.get<DispatchFileItem[]>(this.url+'/'+id+'/files');}
 uploadFile(id:string,file:File){
  const body=new FormData();body.append('file',file,file.name);
  return this.http.post<DispatchFileItem>(this.url+'/'+id+'/files',body);
 }
 downloadFile(id:string,fileId:string){
  return this.http.get(this.url+'/'+id+'/files/'+fileId,{responseType:'blob'});
 }
}
