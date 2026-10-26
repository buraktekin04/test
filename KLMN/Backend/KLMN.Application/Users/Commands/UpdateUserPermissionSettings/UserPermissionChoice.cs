namespace KLMN.Application.Users.Commands.UpdateUserPermissionSettings;
/// <summary>null rol devral, true açık izin, false açık ret.</summary>
public sealed record UserPermissionChoice(Guid PermissionId,bool? IsGranted);
