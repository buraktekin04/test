using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Exceptions;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Queries.GetReasonForExamUnusedHcAdmissionTypeById;
/// <summary>Silinmemiş ilişkiyi ve xmin bilgisini getirir.</summary>
public sealed class GetReasonForExamUnusedHcAdmissionTypeByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetReasonForExamUnusedHcAdmissionTypeByIdQuery,ReasonForExamUnusedHcAdmissionTypeResponse>
{
 public async Task<ReasonForExamUnusedHcAdmissionTypeResponse> Handle(GetReasonForExamUnusedHcAdmissionTypeByIdQuery r,CancellationToken ct)=>
 await db.ReasonForExamUnusedHcAdmissionTypes.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&x.Id==r.Id)
    .Select(x=>new ReasonForExamUnusedHcAdmissionTypeResponse(
 x.Id,x.ReasonForExaminationDefId,x.Reason.Description,x.Reason.Code,
 x.HcAdmissionTypeDefId,x.AdmissionType.Name,x.IsActive,x.Version,x.CreatedDate
 )).SingleOrDefaultAsync(ct)??throw new ReasonForExamUnusedHcAdmissionTypeNotFoundException();
}
