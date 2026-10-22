using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisBranches.Commands.Common;


using MediatR;
namespace KLMN.Application.SYYDiagnosisBranches.Commands.DeleteSYYDiagnosisBranchDef;
/// <summary>SYY Gereken Branşlar Delete handler; interceptor audit ve soft-delete uygular.</summary>
public sealed class DeleteSYYDiagnosisBranchDefCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteSYYDiagnosisBranchDefCommand>
{
 public async Task Handle(DeleteSYYDiagnosisBranchDefCommand r,CancellationToken ct)
 {
 var item=await SYYDiagnosisBranchDefRules.LoadAsync(db,r.Id,r.Version,ct);
 db.SYYDiagnosisBranchDefs.Remove(item);await db.SaveChangesAsync(ct);
 }
}
