using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisRequireds.Commands.Common;
using KLMN.Application.SYYDiagnosisRequireds.Responses;

using MediatR;
namespace KLMN.Application.SYYDiagnosisRequireds.Commands.ChangeStatusSYYDiagnosisRequired;
/// <summary>SYY Onay Gerektiren Maddeler — ChangeStatus ve tek SaveChanges.</summary>
public sealed class ChangeStatusSYYDiagnosisRequiredCommandHandler(IKLMNDbContext db):
 IRequestHandler<ChangeStatusSYYDiagnosisRequiredCommand,SYYDiagnosisRequiredMutationResponse>
{
 public async Task<SYYDiagnosisRequiredMutationResponse> Handle(ChangeStatusSYYDiagnosisRequiredCommand r,CancellationToken ct)
 {
 var item=await SYYDiagnosisRequiredRules.LoadAsync(db,r.Id,r.Version,ct);
 item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
