using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcDecisions.Common;


using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.HcUnusedReasons.Queries.GetHcUnusedReasonForExamOptions;
/// <summary>İlişkileri sunucu tarafında filtreler veya FK seçenekleriyle döndürür.</summary>
public sealed class GetHcUnusedReasonForExamOptionsQueryHandler(IKLMNDbContext db):IRequestHandler<GetHcUnusedReasonForExamOptionsQuery,HcDecisionRelationOptions>
{
 public async Task<HcDecisionRelationOptions> Handle(GetHcUnusedReasonForExamOptionsQuery r,CancellationToken ct)
 {
    var decisions=await db.HcDecisionDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedDecisionId))
        .OrderBy(x=>x.Code).Select(x=>new HcDecisionOption(x.Id,x.Code+" - "+x.Name,x.IsActive)).ToListAsync(ct);
       var refs=await db.ReasonForExaminationDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==r.SelectedReferenceId))
        .OrderBy(x=>x.Description).Select(x=>new HcDecisionOption(x.Id,x.Description,x.IsActive)).ToListAsync(ct);
       return new HcDecisionRelationOptions(decisions,refs);
 }
}
