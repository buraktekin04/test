using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClassGroupRelations.Exceptions;
using KLMN.Application.MilitaryClassGroupRelations.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryClassGroupRelations.Queries.GetMilitaryClassGroupRelationById;

/// <summary>Soft-delete edilmemiş ilişkiyi 404 korumasıyla getirir.</summary>
public sealed class GetMilitaryClassGroupRelationByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryClassGroupRelationByIdQuery, MilitaryClassGroupRelationResponse>
{
    /// <summary>Güncel ilişki detayını ve sürümünü döndürür.</summary>
    public async Task<MilitaryClassGroupRelationResponse> Handle(GetMilitaryClassGroupRelationByIdQuery request, CancellationToken ct) =>
        await db.MilitaryClassGroupRelations.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(MilitaryClassGroupRelationResponse.Projection)
            .SingleOrDefaultAsync(ct) ?? throw new MilitaryClassGroupRelationNotFoundException();
}
