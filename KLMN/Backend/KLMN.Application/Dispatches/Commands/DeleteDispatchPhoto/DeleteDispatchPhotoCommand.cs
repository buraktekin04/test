using KLMN.Application.Dispatches.Responses;
using MediatR;

namespace KLMN.Application.Dispatches.Commands.DeleteDispatchPhoto;

/// <summary>Fotoğrafı, sevk ve diğer belgeleri koruyarak temizler.</summary>
public sealed record DeleteDispatchPhotoCommand(Guid Id,uint Version):IRequest<DispatchMutationResponse>;
