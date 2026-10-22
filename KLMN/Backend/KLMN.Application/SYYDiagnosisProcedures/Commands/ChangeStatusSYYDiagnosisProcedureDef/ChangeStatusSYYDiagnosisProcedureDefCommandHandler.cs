using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisProcedures.Commands.Common;
using KLMN.Application.SYYDiagnosisProcedures.Responses;

using MediatR;
namespace KLMN.Application.SYYDiagnosisProcedures.Commands.ChangeStatusSYYDiagnosisProcedureDef;
/// <summary>SYY Gereken Tetkikler ChangeStatus handler; interceptor audit ve soft-delete uygular.</summary>
public sealed class ChangeStatusSYYDiagnosisProcedureDefCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusSYYDiagnosisProcedureDefCommand,SYYDiagnosisProcedureDefMutationResponse>
{
 public async Task<SYYDiagnosisProcedureDefMutationResponse> Handle(ChangeStatusSYYDiagnosisProcedureDefCommand r,CancellationToken ct)
 {
 
 var item=await SYYDiagnosisProcedureDefRules.LoadAsync(db,r.Id,r.Version,ct);
 item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
