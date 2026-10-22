using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisProcedures.Commands.Common;
using KLMN.Application.SYYDiagnosisProcedures.Responses;

using MediatR;
namespace KLMN.Application.SYYDiagnosisProcedures.Commands.UpdateSYYDiagnosisProcedureDef;
/// <summary>SYY Gereken Tetkikler Update handler; interceptor audit ve soft-delete uygular.</summary>
public sealed class UpdateSYYDiagnosisProcedureDefCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateSYYDiagnosisProcedureDefCommand,SYYDiagnosisProcedureDefMutationResponse>
{
 public async Task<SYYDiagnosisProcedureDefMutationResponse> Handle(UpdateSYYDiagnosisProcedureDefCommand r,CancellationToken ct)
 {
 
 var item=await SYYDiagnosisProcedureDefRules.LoadAsync(db,r.Id,r.Version,ct);
 var fields=new SYYDiagnosisProcedureDefFields(r.SYYDiagnosisDefId,r.ProcedureDefId,r.StartingAge,r.EndingAge,r.Gender);
 await SYYDiagnosisProcedureDefRules.EnsureAsync(db,fields,item.Id,ct,item);
 item.SYYDiagnosisDefId=r.SYYDiagnosisDefId;
item.ProcedureDefId=r.ProcedureDefId;
item.StartingAge=r.StartingAge;
item.EndingAge=r.EndingAge;
item.Gender=r.Gender;
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
