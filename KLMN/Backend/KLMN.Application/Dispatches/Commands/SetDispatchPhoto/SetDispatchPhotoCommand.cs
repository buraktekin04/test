using KLMN.Application.Dispatches.Responses;
using MediatR;

namespace KLMN.Application.Dispatches.Commands.SetDispatchPhoto;

/// <summary>Kamera veya resim yükleyicisinden gelen kişinin fotoğrafını ilişkilendirir.</summary>
public sealed record SetDispatchPhotoCommand(Guid Id,string ContentType,byte[] Content):IRequest<DispatchMutationResponse>;
