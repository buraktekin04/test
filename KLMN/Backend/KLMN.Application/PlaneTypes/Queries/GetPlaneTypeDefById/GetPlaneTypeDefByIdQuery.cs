using KLMN.Application.PlaneTypes.Responses;
using MediatR;
namespace KLMN.Application.PlaneTypes.Queries.GetPlaneTypeDefById;
/// <summary>Uçuşa Uçak Tipi Tanımları tek kayıt sorgusu.</summary>
public sealed record GetPlaneTypeDefByIdQuery(Guid Id):IRequest<PlaneTypeDefResponse>;
