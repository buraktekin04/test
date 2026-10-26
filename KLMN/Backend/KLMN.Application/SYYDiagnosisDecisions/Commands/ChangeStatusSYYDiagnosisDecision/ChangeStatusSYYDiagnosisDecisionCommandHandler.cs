using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisDecisions.Commands.Common;

using KLMN.Application.SYYDiagnosisDecisions.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnosisDecisions.Commands.ChangeStatusSYYDiagnosisDecision;
/// <summary>ChangeStatus istatistik işlemi; BaseEntity audit ve xmin korunur.</summary>
public sealed class ChangeStatusSYYDiagnosisDecisionCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusSYYDiagnosisDecisionCommand,SYYDiagnosisDecisionMutationResponse>
{
 public async Task<SYYDiagnosisDecisionMutationResponse> Handle(ChangeStatusSYYDiagnosisDecisionCommand r,CancellationToken ct)
 {
    var item=await SYYDiagnosisDecisionRules.LoadAsync(db,r.Id,r.Version,ct);
     if(r.IsActive)await SYYDiagnosisDecisionRules.ValidateActiveReferencesAsync(db,item,ct);
     item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
