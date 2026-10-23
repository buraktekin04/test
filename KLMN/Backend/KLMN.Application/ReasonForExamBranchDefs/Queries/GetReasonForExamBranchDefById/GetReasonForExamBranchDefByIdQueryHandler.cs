using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamBranchDefs.Exceptions;
using KLMN.Application.ReasonForExamBranchDefs.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExamBranchDefs.Queries.GetReasonForExamBranchDefById;
/// <summary>Silinmemiş ilişkiyi ve xmin bilgisini getirir.</summary>
public sealed class GetReasonForExamBranchDefByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetReasonForExamBranchDefByIdQuery,ReasonForExamBranchDefResponse>
{
 public async Task<ReasonForExamBranchDefResponse> Handle(GetReasonForExamBranchDefByIdQuery r,CancellationToken ct)=>
 await db.ReasonForExamBranchDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&x.Id==r.Id)
    .Select(x=>new ReasonForExamBranchDefResponse(
 x.Id,x.ReasonForExaminationDefId,x.Reason.Description,x.Reason.Code,
 x.BranchDefId,x.Branch.Name,x.IsActive,x.Version,x.CreatedDate
 ,x.StartingAge,x.EndingAge,x.Gender)).SingleOrDefaultAsync(ct)??throw new ReasonForExamBranchDefNotFoundException();
}
