using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisBranches.Commands.Common;
using KLMN.Application.SYYDiagnosisBranches.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
namespace KLMN.Application.SYYDiagnosisBranches.Commands.CreateSYYDiagnosisBranchDef;
/// <summary>SYY Gereken Branşlar Create handler; interceptor audit ve soft-delete uygular.</summary>
public sealed class CreateSYYDiagnosisBranchDefCommandHandler(IKLMNDbContext db):IRequestHandler<CreateSYYDiagnosisBranchDefCommand,SYYDiagnosisBranchDefMutationResponse>
{
 public async Task<SYYDiagnosisBranchDefMutationResponse> Handle(CreateSYYDiagnosisBranchDefCommand r,CancellationToken ct)
 {
 
 var fields=new SYYDiagnosisBranchDefFields(r.SYYDiagnosisDefId,r.BranchDefId,r.StartingAge,r.EndingAge,r.Gender);
 await SYYDiagnosisBranchDefRules.EnsureAsync(db,fields,null,ct);
 var item=new SYYDiagnosisBranchDef{SYYDiagnosisDefId=r.SYYDiagnosisDefId,BranchDefId=r.BranchDefId,StartingAge=r.StartingAge,EndingAge=r.EndingAge,Gender=r.Gender};
 await db.SYYDiagnosisBranchDefs.AddAsync(item,ct);
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
