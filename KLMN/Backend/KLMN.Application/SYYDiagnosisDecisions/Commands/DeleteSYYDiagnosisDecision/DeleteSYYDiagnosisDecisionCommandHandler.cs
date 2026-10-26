using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisDecisions.Commands.Common;


using MediatR;
namespace KLMN.Application.SYYDiagnosisDecisions.Commands.DeleteSYYDiagnosisDecision;
/// <summary>Delete istatistik işlemi; BaseEntity audit ve xmin korunur.</summary>
public sealed class DeleteSYYDiagnosisDecisionCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteSYYDiagnosisDecisionCommand>
{
 public async Task Handle(DeleteSYYDiagnosisDecisionCommand r,CancellationToken ct)
 {
    var item=await SYYDiagnosisDecisionRules.LoadAsync(db,r.Id,r.Version,ct);
     db.SYYDiagnosisDecisions.Remove(item);await db.SaveChangesAsync(ct);
 }
}
