using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisBranches.Commands.Common;
using KLMN.Application.SYYDiagnosisBranches.Responses;

using MediatR;
namespace KLMN.Application.SYYDiagnosisBranches.Commands.ChangeStatusSYYDiagnosisBranchDef;
/// <summary>SYY Gereken Branşlar ChangeStatus handler; interceptor audit ve soft-delete uygular.</summary>
public sealed class ChangeStatusSYYDiagnosisBranchDefCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusSYYDiagnosisBranchDefCommand,SYYDiagnosisBranchDefMutationResponse>
{
 public async Task<SYYDiagnosisBranchDefMutationResponse> Handle(ChangeStatusSYYDiagnosisBranchDefCommand r,CancellationToken ct)
 {
 
 var item=await SYYDiagnosisBranchDefRules.LoadAsync(db,r.Id,r.Version,ct);
 item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
