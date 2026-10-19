using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Branches.Exceptions;
using KLMN.Application.Branches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Branches.Queries.GetBranchDefById;

/// <summary>Karar detayını seçilen GUID ile sorgular.</summary>
public sealed class GetBranchDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetBranchDefByIdQuery, BranchDefResponse>
{
    /// <summary>Silinmiş veya olmayan branşı 404 olarak ele alır.</summary>
    public async Task<BranchDefResponse> Handle(GetBranchDefByIdQuery request, CancellationToken ct) =>
        await db.BranchDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(BranchDefResponse.Projection).SingleOrDefaultAsync(ct)
            ?? throw new BranchNotFoundException();
}
