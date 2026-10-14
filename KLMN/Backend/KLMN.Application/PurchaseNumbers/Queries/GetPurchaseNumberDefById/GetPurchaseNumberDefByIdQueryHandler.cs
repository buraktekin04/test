using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.PurchaseNumbers.Exceptions;
using KLMN.Application.PurchaseNumbers.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.PurchaseNumbers.Queries.GetPurchaseNumberDefById;

public sealed class GetPurchaseNumberDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetPurchaseNumberDefByIdQuery, PurchaseNumberDefResponse>
{
    public async Task<PurchaseNumberDefResponse> Handle(GetPurchaseNumberDefByIdQuery request, CancellationToken ct) =>
        await db.PurchaseNumberDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(PurchaseNumberDefResponse.Projection)
            .SingleOrDefaultAsync(ct) ?? throw new PurchaseNumberNotFoundException();
}
