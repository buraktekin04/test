using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisDecisions.Commands.Common;

using KLMN.Application.SYYDiagnosisDecisions.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnosisDecisions.Commands.UpdateSYYDiagnosisDecision;
/// <summary>Update istatistik işlemi; BaseEntity audit ve xmin korunur.</summary>
public sealed class UpdateSYYDiagnosisDecisionCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateSYYDiagnosisDecisionCommand,SYYDiagnosisDecisionMutationResponse>
{
 public async Task<SYYDiagnosisDecisionMutationResponse> Handle(UpdateSYYDiagnosisDecisionCommand r,CancellationToken ct)
 {
    var item=await SYYDiagnosisDecisionRules.LoadAsync(db,r.Id,r.Version,ct);
     await SYYDiagnosisDecisionRules.ValidateReferencesAsync(db,r.SYYDiagnosisDefId,r.HcDecisionDefId,r.HcAdmissionTypeDefId,r.MilitaryClassDefId,ct,item);
     await SYYDiagnosisDecisionRules.EnsureUniqueAsync(db,r.SYYDiagnosisDefId,r.HcDecisionDefId,r.HcAdmissionTypeDefId,r.MilitaryClassDefId,item.Id,ct);
     item.SYYDiagnosisDefId=r.SYYDiagnosisDefId;item.HcDecisionDefId=r.HcDecisionDefId;item.HcAdmissionTypeDefId=r.HcAdmissionTypeDefId;
     item.MilitaryClassDefId=r.MilitaryClassDefId;item.Amount=r.Amount;
     await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
