using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExaminations.Responses;
using KLMN.Application.ReasonForExaminations.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExaminations.Queries.GetReasonForExaminationDefById;
/// <summary>Muayene sebebi detay ve ilişkilerinin tamamını tek mantıksal sorguda okur.</summary>
public sealed class GetReasonForExaminationDefByIdQueryHandler(IKLMNDbContext db)
 : IRequestHandler<GetReasonForExaminationDefByIdQuery,ReasonForExaminationDefResponse>
{
 public async Task<ReasonForExaminationDefResponse> Handle(GetReasonForExaminationDefByIdQuery r,CancellationToken ct)
 {
   var x=await db.ReasonForExaminationDefs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>x.Id==r.Id&&!x.IsDeleted).Include(x=>x.ReportTypes).ThenInclude(x=>x.ReportType)
            .Include(x=>x.UnusedAdmissionTypes).ThenInclude(x=>x.AdmissionType)
            .Include(x=>x.Procedures).ThenInclude(x=>x.Procedure)
            .Include(x=>x.Branches).ThenInclude(x=>x.Branch).AsSplitQuery().SingleOrDefaultAsync(ct)
   ?? throw new ReasonForExaminationNotFoundException();
   return ReasonForExaminationDefResponse.From(x);
 }
}
