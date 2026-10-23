using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ICD10Diagnostics.Commands.Common;
using KLMN.Application.ICD10Diagnostics.Responses;

using MediatR;
namespace KLMN.Application.ICD10Diagnostics.Commands.UpdateICD10Diagnostic;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları — Update işlemi, soft-delete/audit/xmin mevcut altyapıda.</summary>
public sealed class UpdateICD10DiagnosticCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateICD10DiagnosticCommand,ICD10DiagnosticMutationResponse>
{
 public async Task<ICD10DiagnosticMutationResponse> Handle(UpdateICD10DiagnosticCommand r,CancellationToken ct)
 {
 
 var item=await ICD10DiagnosticRules.LoadAsync(db,r.Id,r.Version,ct);
 await ICD10DiagnosticRules.ValidateReferencesAsync(db,r.ICD10DefId,r.SYYDiagnosisDefId,r.HcAdmissionTypeDefId,ct,item);
 await ICD10DiagnosticRules.EnsureUniqueAsync(db,r.ICD10DefId,r.SYYDiagnosisDefId,r.HcAdmissionTypeDefId,r.Id,ct);
 item.ICD10DefId=r.ICD10DefId;item.SYYDiagnosisDefId=r.SYYDiagnosisDefId;
 item.HcAdmissionTypeDefId=r.HcAdmissionTypeDefId;item.Amount=r.Amount;
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
