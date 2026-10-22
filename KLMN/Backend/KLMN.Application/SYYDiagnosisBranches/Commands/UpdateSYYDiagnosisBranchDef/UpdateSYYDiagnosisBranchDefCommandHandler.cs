using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisBranches.Commands.Common;
using KLMN.Application.SYYDiagnosisBranches.Responses;

using MediatR;
namespace KLMN.Application.SYYDiagnosisBranches.Commands.UpdateSYYDiagnosisBranchDef;
/// <summary>SYY Gereken Branşlar Update handler; interceptor audit ve soft-delete uygular.</summary>
public sealed class UpdateSYYDiagnosisBranchDefCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateSYYDiagnosisBranchDefCommand,SYYDiagnosisBranchDefMutationResponse>
{
 public async Task<SYYDiagnosisBranchDefMutationResponse> Handle(UpdateSYYDiagnosisBranchDefCommand r,CancellationToken ct)
 {
 
 var item=await SYYDiagnosisBranchDefRules.LoadAsync(db,r.Id,r.Version,ct);
 var fields=new SYYDiagnosisBranchDefFields(r.SYYDiagnosisDefId,r.BranchDefId,r.StartingAge,r.EndingAge,r.Gender);
 await SYYDiagnosisBranchDefRules.EnsureAsync(db,fields,item.Id,ct,item);
 item.SYYDiagnosisDefId=r.SYYDiagnosisDefId;
item.BranchDefId=r.BranchDefId;
item.StartingAge=r.StartingAge;
item.EndingAge=r.EndingAge;
item.Gender=r.Gender;
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
