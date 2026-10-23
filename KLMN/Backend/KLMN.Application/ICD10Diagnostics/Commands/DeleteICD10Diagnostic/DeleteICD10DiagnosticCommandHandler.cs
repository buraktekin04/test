using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ICD10Diagnostics.Commands.Common;


using MediatR;
namespace KLMN.Application.ICD10Diagnostics.Commands.DeleteICD10Diagnostic;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları — Delete işlemi, soft-delete/audit/xmin mevcut altyapıda.</summary>
public sealed class DeleteICD10DiagnosticCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteICD10DiagnosticCommand>
{
 public async Task Handle(DeleteICD10DiagnosticCommand r,CancellationToken ct)
 {
 var item=await ICD10DiagnosticRules.LoadAsync(db,r.Id,r.Version,ct);
 
 db.ICD10Diagnostics.Remove(item);
 await db.SaveChangesAsync(ct);
 }
}
