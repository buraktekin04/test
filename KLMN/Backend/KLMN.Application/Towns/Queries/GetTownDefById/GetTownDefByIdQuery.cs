using MediatR;
using KLMN.Application.Towns.Responses;
namespace KLMN.Application.Towns.Queries.GetTownDefById;
/// <summary>İlçe tanımına ait sorgu, yanıt veya iş kuralı bileşeni.</summary>
public sealed record GetTownDefByIdQuery(Guid Id) : IRequest<TownDefResponse>;
