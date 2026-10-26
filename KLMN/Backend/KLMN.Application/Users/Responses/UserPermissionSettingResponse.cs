namespace KLMN.Application.Users.Responses;
/// <summary>Tek izin için rol etkisi, kullanıcı override'ı ve nihai karar.</summary>
public sealed record UserPermissionSettingResponse(Guid PermissionId,string Code,string Name,string Module,int SortOrder,
 bool GrantedByRole,bool? IsGrantedOverride,bool Effective);
