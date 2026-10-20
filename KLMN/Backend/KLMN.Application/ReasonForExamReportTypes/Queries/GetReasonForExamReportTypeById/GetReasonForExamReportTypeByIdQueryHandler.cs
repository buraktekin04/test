using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamReportTypes.Exceptions;
using KLMN.Application.ReasonForExamReportTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExamReportTypes.Queries.GetReasonForExamReportTypeById;
/// <summary>Silinmemiş ilişkiyi ve xmin bilgisini getirir.</summary>
public sealed class GetReasonForExamReportTypeByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetReasonForExamReportTypeByIdQuery,ReasonForExamReportTypeResponse>
{
 public async Task<ReasonForExamReportTypeResponse> Handle(GetReasonForExamReportTypeByIdQuery r,CancellationToken ct)=>
 await db.ReasonForExamReportTypes.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&x.Id==r.Id)
    .Select(ReasonForExamReportTypeResponse.Projection).SingleOrDefaultAsync(ct)??throw new ReasonForExamReportTypeNotFoundException();
}
