using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedReasons.Responses;

using KLMN.Application.HcUnusedReasons.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.HcUnusedReasons.Queries.GetHcUnusedReasonForExamById;
/// <summary>İlişkileri sunucu tarafında filtreler veya FK seçenekleriyle döndürür.</summary>
public sealed class GetHcUnusedReasonForExamByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetHcUnusedReasonForExamByIdQuery,HcUnusedReasonForExamResponse>
{
 public async Task<HcUnusedReasonForExamResponse> Handle(GetHcUnusedReasonForExamByIdQuery r,CancellationToken ct)
 {
    return await db.HcUnusedReasonForExams.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&x.Id==r.Id)
        .Select(x=>new HcUnusedReasonForExamResponse(x.Id,x.HcDecisionDefId,x.Decision.Name,x.ReasonForExaminationDefId,x.Reason.Description,x.IsActive,x.Version,x.CreatedDate)).SingleOrDefaultAsync(ct)??throw new HcUnusedReasonForExamNotFoundException();
 }
}
