using KLMN.Application.Users.Responses;
using MediatR;
namespace KLMN.Application.Users.Commands.UpdateUserPermissionSettings;
/// <summary>Kişisel izin değişikliklerini tek DB transaction ve xmin korumasıyla kaydeder.</summary>
public sealed record UpdateUserPermissionSettingsCommand(Guid UserId,uint Version,
 IReadOnlyCollection<UserPermissionChoice> Choices):IRequest<UserMutationResponse>;
