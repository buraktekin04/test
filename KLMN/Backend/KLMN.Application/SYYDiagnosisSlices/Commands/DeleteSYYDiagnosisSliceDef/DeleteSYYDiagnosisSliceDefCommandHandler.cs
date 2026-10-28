using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.SYYDiagnosisSlices.Commands.Common;
 
 
 using MediatR;
 namespace KLMN.Application.SYYDiagnosisSlices.Commands.DeleteSYYDiagnosisSliceDef;
 /// <summary>SYY Dilim Tanımları Delete iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class DeleteSYYDiagnosisSliceDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<DeleteSYYDiagnosisSliceDefCommand>
 {
 public async Task Handle(DeleteSYYDiagnosisSliceDefCommand r,CancellationToken ct){
 var item=await SYYDiagnosisSliceDefRules.LoadAsync(db,r.Id,r.Version,ct);
 await SYYDiagnosisSliceDefRules.EnsureUnusedAsync(db,item.Id,ct);
 db.SYYDiagnosisSliceDefs.Remove(item);await db.SaveChangesAsync(ct);
 }
 }
