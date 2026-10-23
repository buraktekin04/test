using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ICD10Diagnostics.Commands.Common;
using KLMN.Application.ICD10Diagnostics.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
namespace KLMN.Application.ICD10Diagnostics.Commands.CreateICD10Diagnostic;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları — Create işlemi, soft-delete/audit/xmin mevcut altyapıda.</summary>
public sealed class CreateICD10DiagnosticCommandHandler(IKLMNDbContext db):IRequestHandler<CreateICD10DiagnosticCommand,ICD10DiagnosticMutationResponse>
{
 public async Task<ICD10DiagnosticMutationResponse> Handle(CreateICD10DiagnosticCommand r,CancellationToken ct)
 {
 
 await ICD10DiagnosticRules.ValidateReferencesAsync(db,r.ICD10DefId,r.SYYDiagnosisDefId,r.HcAdmissionTypeDefId,ct);
 await ICD10DiagnosticRules.EnsureUniqueAsync(db,r.ICD10DefId,r.SYYDiagnosisDefId,r.HcAdmissionTypeDefId,null,ct);
 var item=new ICD10Diagnostic{ICD10DefId=r.ICD10DefId,SYYDiagnosisDefId=r.SYYDiagnosisDefId,
  HcAdmissionTypeDefId=r.HcAdmissionTypeDefId,Amount=r.Amount};
 await db.ICD10Diagnostics.AddAsync(item,ct);
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
