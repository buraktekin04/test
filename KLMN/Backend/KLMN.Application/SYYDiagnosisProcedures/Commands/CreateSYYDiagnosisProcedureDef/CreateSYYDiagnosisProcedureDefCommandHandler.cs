using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisProcedures.Commands.Common;
using KLMN.Application.SYYDiagnosisProcedures.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
namespace KLMN.Application.SYYDiagnosisProcedures.Commands.CreateSYYDiagnosisProcedureDef;
/// <summary>SYY Gereken Tetkikler Create handler; interceptor audit ve soft-delete uygular.</summary>
public sealed class CreateSYYDiagnosisProcedureDefCommandHandler(IKLMNDbContext db):IRequestHandler<CreateSYYDiagnosisProcedureDefCommand,SYYDiagnosisProcedureDefMutationResponse>
{
 public async Task<SYYDiagnosisProcedureDefMutationResponse> Handle(CreateSYYDiagnosisProcedureDefCommand r,CancellationToken ct)
 {
 
 var fields=new SYYDiagnosisProcedureDefFields(r.SYYDiagnosisDefId,r.ProcedureDefId,r.StartingAge,r.EndingAge,r.Gender);
 await SYYDiagnosisProcedureDefRules.EnsureAsync(db,fields,null,ct);
 var item=new SYYDiagnosisProcedureDef{SYYDiagnosisDefId=r.SYYDiagnosisDefId,ProcedureDefId=r.ProcedureDefId,StartingAge=r.StartingAge,EndingAge=r.EndingAge,Gender=r.Gender};
 await db.SYYDiagnosisProcedureDefs.AddAsync(item,ct);
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
