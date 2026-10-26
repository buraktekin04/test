using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcDecisions.Exceptions;
using KLMN.Application.HcDecisions.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.HcDecisions.Queries.GetHcDecisionDefById;
/// <summary>Kararları sunucuda sorgular; alt ilişki adları yalnız getirilen sayfaya yüklenir.</summary>
public sealed class GetHcDecisionDefByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetHcDecisionDefByIdQuery,HcDecisionDefResponse>
{
    public async Task<HcDecisionDefResponse> Handle(GetHcDecisionDefByIdQuery r,CancellationToken ct)
    {
        var item=await db.HcDecisionDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&x.Id==r.Id)
          .Include(x=>x.UnusedReasons).ThenInclude(x=>x.Reason)
 .Include(x=>x.UnusedAdmissionTypes).ThenInclude(x=>x.AdmissionType)
 .Include(x=>x.UnusedDecisions).ThenInclude(x=>x.UnusedDecision)
 .Include(x=>x.ConnectedDecisions).ThenInclude(x=>x.ConnectedDecisionDef).AsSplitQuery().SingleOrDefaultAsync(ct)??throw new HcDecisionDefNotFoundException();
        return HcDecisionDefResponse.From(item);
    }
}
