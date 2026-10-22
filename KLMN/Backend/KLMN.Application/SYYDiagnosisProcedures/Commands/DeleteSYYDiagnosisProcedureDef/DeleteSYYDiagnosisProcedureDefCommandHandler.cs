using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisProcedures.Commands.Common;


using MediatR;
namespace KLMN.Application.SYYDiagnosisProcedures.Commands.DeleteSYYDiagnosisProcedureDef;
/// <summary>SYY Gereken Tetkikler Delete handler; interceptor audit ve soft-delete uygular.</summary>
public sealed class DeleteSYYDiagnosisProcedureDefCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteSYYDiagnosisProcedureDefCommand>
{
 public async Task Handle(DeleteSYYDiagnosisProcedureDefCommand r,CancellationToken ct)
 {
 var item=await SYYDiagnosisProcedureDefRules.LoadAsync(db,r.Id,r.Version,ct);
 db.SYYDiagnosisProcedureDefs.Remove(item);await db.SaveChangesAsync(ct);
 }
}
