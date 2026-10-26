using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisDecisions.Commands.Common;
using KLMN.Domain.Entities.Admissions;
using KLMN.Application.SYYDiagnosisDecisions.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnosisDecisions.Commands.CreateSYYDiagnosisDecision;
/// <summary>Create istatistik işlemi; BaseEntity audit ve xmin korunur.</summary>
public sealed class CreateSYYDiagnosisDecisionCommandHandler(IKLMNDbContext db):IRequestHandler<CreateSYYDiagnosisDecisionCommand,SYYDiagnosisDecisionMutationResponse>
{
 public async Task<SYYDiagnosisDecisionMutationResponse> Handle(CreateSYYDiagnosisDecisionCommand r,CancellationToken ct)
 {
    await SYYDiagnosisDecisionRules.ValidateReferencesAsync(db,r.SYYDiagnosisDefId,r.HcDecisionDefId,r.HcAdmissionTypeDefId,r.MilitaryClassDefId,ct);
     await SYYDiagnosisDecisionRules.EnsureUniqueAsync(db,r.SYYDiagnosisDefId,r.HcDecisionDefId,r.HcAdmissionTypeDefId,r.MilitaryClassDefId,null,ct);
     var item=new SYYDiagnosisDecision{SYYDiagnosisDefId=r.SYYDiagnosisDefId,HcDecisionDefId=r.HcDecisionDefId,HcAdmissionTypeDefId=r.HcAdmissionTypeDefId,MilitaryClassDefId=r.MilitaryClassDefId,Amount=r.Amount};
     await db.SYYDiagnosisDecisions.AddAsync(item,ct);await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
