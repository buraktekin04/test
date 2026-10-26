/** Sunucunun rol + kullanıcı override önceliğine göre ürettiği izin satırı. */
export interface UserPermissionSetting {
  permissionId:string;code:string;name:string;module:string;sortOrder:number;
  grantedByRole:boolean;isGrantedOverride:boolean|null;effective:boolean;
}
export interface UserPermissionSettings {
  userId:string;version:number;isAdmin:boolean;permissions:UserPermissionSetting[];
}
export interface UpdateUserPermissionSettingsRequest {
  userId:string;version:number;choices:{permissionId:string;isGranted:boolean|null}[];
}
