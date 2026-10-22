using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.SYYDiagnoses.Commands.Common;
 
 
 using MediatR;
 namespace KLMN.Application.SYYDiagnoses.Commands.DeleteSYYDiagnosisDef;
 /// <summary>SYY Tanı Listesi (M/D/F) Delete iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class DeleteSYYDiagnosisDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<DeleteSYYDiagnosisDefCommand>
 {
 public async Task Handle(DeleteSYYDiagnosisDefCommand r,CancellationToken ct){
 var item=await SYYDiagnosisDefRules.LoadAsync(db,r.Id,r.Version,ct);
 await SYYDiagnosisDefRules.EnsureNoChildrenAsync(db,item.Id,false,ct);
 db.SYYDiagnosisDefs.Remove(item);await db.SaveChangesAsync(ct);
 }
 }
