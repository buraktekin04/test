using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.RankGroups.Exceptions;
using KLMN.Application.RankGroups.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.RankGroups.Queries.GetRankGroupDefById;

/// <summary>Kayıt detayını seçilen GUID ile sorgular.</summary>
public sealed class GetRankGroupDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetRankGroupDefByIdQuery, RankGroupDefResponse>
{
    /// <summary>Silinmiş veya olmayan rütbe grubuı 404 olarak ele alır.</summary>
    public async Task<RankGroupDefResponse> Handle(GetRankGroupDefByIdQuery request, CancellationToken ct) =>
        await db.RankGroupDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x =>
        new RankGroupDefResponse
        {
            Id = x.Id, Code = x.Code, Name = x.Name, ShortName = x.ShortName,
            ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, IsActive = x.IsActive,
            CreatedDate = x.CreatedDate, CreatedBy = x.CreatedBy,
            UpdatedDate = x.UpdatedDate, UpdatedBy = x.UpdatedBy, Version = x.Version
        }).SingleOrDefaultAsync(ct)
            ?? throw new RankGroupNotFoundException();
}
