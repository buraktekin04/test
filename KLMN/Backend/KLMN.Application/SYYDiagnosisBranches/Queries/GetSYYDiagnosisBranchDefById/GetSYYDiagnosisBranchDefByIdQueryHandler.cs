using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisBranches.Responses;
using KLMN.Application.SYYDiagnosisBranches.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisBranches.Queries.GetSYYDiagnosisBranchDefById;
public sealed class GetSYYDiagnosisBranchDefByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetSYYDiagnosisBranchDefByIdQuery,SYYDiagnosisBranchDefResponse>
{
 public async Task<SYYDiagnosisBranchDefResponse> Handle(GetSYYDiagnosisBranchDefByIdQuery r,CancellationToken ct)=>
 await db.SYYDiagnosisBranchDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>x.Id==r.Id&&!x.IsDeleted)
  .Select(x=>new SYYDiagnosisBranchDefResponse(x.Id,x.SYYDiagnosisDefId,x.BranchDefId,x.Diagnosis.Name,x.Branch.Name,x.StartingAge,x.EndingAge,x.Gender,x.IsActive,x.Version,x.CreatedDate)).SingleOrDefaultAsync(ct)??throw new SYYDiagnosisBranchDefNotFoundException();
}
