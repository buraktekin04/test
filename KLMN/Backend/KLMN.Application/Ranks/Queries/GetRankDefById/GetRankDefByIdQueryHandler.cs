using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Ranks.Exceptions;
using KLMN.Application.Ranks.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Ranks.Queries.GetRankDefById;

/// <summary>Karar detayını seçilen GUID ile sorgular.</summary>
public sealed class GetRankDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetRankDefByIdQuery, RankDefResponse>
{
    /// <summary>Silinmiş veya olmayan rütbeı 404 olarak ele alır.</summary>
    public async Task<RankDefResponse> Handle(GetRankDefByIdQuery request, CancellationToken ct) =>
        await db.RankDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(RankDefResponse.Projection).SingleOrDefaultAsync(ct)
            ?? throw new RankNotFoundException();
}
