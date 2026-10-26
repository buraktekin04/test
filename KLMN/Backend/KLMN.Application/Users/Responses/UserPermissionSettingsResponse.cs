namespace KLMN.Application.Users.Responses;
/// <summary>Yetki ekranının eşzamanlılık sürümü ve izin seçenekleri.</summary>
public sealed record UserPermissionSettingsResponse(Guid UserId,uint Version,bool IsAdmin,
 IReadOnlyList<UserPermissionSettingResponse> Permissions);
