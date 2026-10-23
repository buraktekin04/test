using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ICD10Diagnostics.Commands.Common;
using KLMN.Application.ICD10Diagnostics.Responses;

using MediatR;
namespace KLMN.Application.ICD10Diagnostics.Commands.ChangeStatusICD10Diagnostic;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları — ChangeStatus işlemi, soft-delete/audit/xmin mevcut altyapıda.</summary>
public sealed class ChangeStatusICD10DiagnosticCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusICD10DiagnosticCommand,ICD10DiagnosticMutationResponse>
{
 public async Task<ICD10DiagnosticMutationResponse> Handle(ChangeStatusICD10DiagnosticCommand r,CancellationToken ct)
 {
 
 var item=await ICD10DiagnosticRules.LoadAsync(db,r.Id,r.Version,ct);
 if(r.IsActive&&!item.IsActive)await ICD10DiagnosticRules.ValidateActiveReferencesAsync(db,item,ct);
 item.IsActive=r.IsActive;
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
