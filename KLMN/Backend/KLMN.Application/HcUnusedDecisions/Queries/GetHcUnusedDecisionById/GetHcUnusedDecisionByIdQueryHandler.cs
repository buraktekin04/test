using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedDecisions.Responses;

using KLMN.Application.HcUnusedDecisions.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.HcUnusedDecisions.Queries.GetHcUnusedDecisionById;
/// <summary>İlişkileri sunucu tarafında filtreler veya FK seçenekleriyle döndürür.</summary>
public sealed class GetHcUnusedDecisionByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetHcUnusedDecisionByIdQuery,HcUnusedDecisionResponse>
{
 public async Task<HcUnusedDecisionResponse> Handle(GetHcUnusedDecisionByIdQuery r,CancellationToken ct)
 {
    return await db.HcUnusedDecisions.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&x.Id==r.Id)
        .Select(x=>new HcUnusedDecisionResponse(x.Id,x.HcDecisionDefId,x.Decision.Name,x.UnusedHcDecisionDefId,x.UnusedDecision.Name,x.IsActive,x.Version,x.CreatedDate)).SingleOrDefaultAsync(ct)??throw new HcUnusedDecisionNotFoundException();
 }
}
