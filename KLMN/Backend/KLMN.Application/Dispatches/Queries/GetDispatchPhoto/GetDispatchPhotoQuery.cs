using KLMN.Application.Dispatches.Responses;
using MediatR;

namespace KLMN.Application.Dispatches.Queries.GetDispatchPhoto;

/// <summary>Tek sevkin başvuran fotoğrafını sorgular.</summary>
public sealed record GetDispatchPhotoQuery(Guid Id):IRequest<DispatchPhotoResponse>;
