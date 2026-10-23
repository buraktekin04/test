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
            .Select(x =>
        new RankDefResponse
        {
            Id = x.Id, Code = x.Code, Name = x.Name,
            ShortName = x.ShortName, Type = x.Type,
            RankGroupId = x.RankGroupId,
            RankGroupName = x.RankGroup == null ? null : x.RankGroup.Name,
            ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, IsActive = x.IsActive,
            CreatedDate = x.CreatedDate, CreatedBy = x.CreatedBy,
            UpdatedDate = x.UpdatedDate, UpdatedBy = x.UpdatedBy, Version = x.Version
        }).SingleOrDefaultAsync(ct)
            ?? throw new RankNotFoundException();
}
