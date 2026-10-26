using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcDecisionConnections.Responses;

using KLMN.Application.HcDecisionConnections.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.HcDecisionConnections.Queries.GetHcConnectedDecisionById;
/// <summary>İlişkileri sunucu tarafında filtreler veya FK seçenekleriyle döndürür.</summary>
public sealed class GetHcConnectedDecisionByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetHcConnectedDecisionByIdQuery,HcConnectedDecisionResponse>
{
 public async Task<HcConnectedDecisionResponse> Handle(GetHcConnectedDecisionByIdQuery r,CancellationToken ct)
 {
    return await db.HcConnectedDecisions.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&x.Id==r.Id)
        .Select(x=>new HcConnectedDecisionResponse(x.Id,x.HcDecisionDefId,x.Decision.Name,x.HcConnectedDecisionDefId,x.ConnectedDecisionDef.Name,x.IsActive,x.Version,x.CreatedDate)).SingleOrDefaultAsync(ct)??throw new HcConnectedDecisionNotFoundException();
 }
}
