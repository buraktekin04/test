using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.SYYDiagnosisTypes.Commands.Common;
 
 
 using MediatR;
 namespace KLMN.Application.SYYDiagnosisTypes.Commands.DeleteSYYDiagnosisTypeDef;
 /// <summary>SYY Tanı Türü Tanımları Delete iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class DeleteSYYDiagnosisTypeDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<DeleteSYYDiagnosisTypeDefCommand>
 {
 public async Task Handle(DeleteSYYDiagnosisTypeDefCommand r,CancellationToken ct){
 var item=await SYYDiagnosisTypeDefRules.LoadAsync(db,r.Id,r.Version,ct);
 await SYYDiagnosisTypeDefRules.EnsureUnusedAsync(db,item.Id,ct);
 db.SYYDiagnosisTypeDefs.Remove(item);await db.SaveChangesAsync(ct);
 }
 }
