using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamProcedureDefs.Exceptions;
using KLMN.Application.ReasonForExamProcedureDefs.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExamProcedureDefs.Queries.GetReasonForExamProcedureDefById;
/// <summary>Silinmemiş ilişkiyi ve xmin bilgisini getirir.</summary>
public sealed class GetReasonForExamProcedureDefByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetReasonForExamProcedureDefByIdQuery,ReasonForExamProcedureDefResponse>
{
 public async Task<ReasonForExamProcedureDefResponse> Handle(GetReasonForExamProcedureDefByIdQuery r,CancellationToken ct)=>
 await db.ReasonForExamProcedureDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&x.Id==r.Id)
    .Select(x=>new ReasonForExamProcedureDefResponse(
 x.Id,x.ReasonForExaminationDefId,x.Reason.Description,x.Reason.Code,
 x.ProcedureDefId,x.Procedure.Name,x.IsActive,x.Version,x.CreatedDate
 ,x.StartingAge,x.EndingAge,x.Gender)).SingleOrDefaultAsync(ct)??throw new ReasonForExamProcedureDefNotFoundException();
}
