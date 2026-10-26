using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcDecisions.Common;


using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.HcUnusedAdmissionTypes.Queries.GetHcUnusedAdmissionTypeOptions;
/// <summary>İlişkileri sunucu tarafında filtreler veya FK seçenekleriyle döndürür.</summary>
public sealed class GetHcUnusedAdmissionTypeOptionsQueryHandler(IKLMNDbContext db):IRequestHandler<GetHcUnusedAdmissionTypeOptionsQuery,HcDecisionRelationOptions>
{
 public async Task<HcDecisionRelationOptions> Handle(GetHcUnusedAdmissionTypeOptionsQuery r,CancellationToken ct)
 {
    var decisions=await db.HcDecisionDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedDecisionId))
        .OrderBy(x=>x.Code).Select(x=>new HcDecisionOption(x.Id,x.Code+" - "+x.Name,x.IsActive)).ToListAsync(ct);
       var refs=await db.HcAdmissionTypeDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedReferenceId))
        .OrderBy(x=>x.Name).Select(x=>new HcDecisionOption(x.Id,x.Name,x.IsActive)).ToListAsync(ct);
       return new HcDecisionRelationOptions(decisions,refs);
 }
}
