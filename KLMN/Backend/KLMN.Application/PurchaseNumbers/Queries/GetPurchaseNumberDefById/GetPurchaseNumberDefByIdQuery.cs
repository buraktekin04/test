using KLMN.Application.PurchaseNumbers.Responses;
using MediatR;

namespace KLMN.Application.PurchaseNumbers.Queries.GetPurchaseNumberDefById;

public sealed record GetPurchaseNumberDefByIdQuery(Guid Id) : IRequest<PurchaseNumberDefResponse>;
