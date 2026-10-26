using KLMN.Application.Users.Responses;
using MediatR;
namespace KLMN.Application.Users.Queries.GetUserPermissionSettings;
/// <summary>Kullanıcının rol izinlerini ve kişisel kararlarını görüntüler.</summary>
public sealed record GetUserPermissionSettingsQuery(Guid UserId):IRequest<UserPermissionSettingsResponse>;
