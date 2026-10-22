using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.SYYDiagnosisSlices.Commands.Common;
 using KLMN.Application.SYYDiagnosisSlices.Responses;
 
 using MediatR;
 namespace KLMN.Application.SYYDiagnosisSlices.Commands.UpdateSYYDiagnosisSliceDef;
 /// <summary>SYY Dilim Tanımları Update iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class UpdateSYYDiagnosisSliceDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<UpdateSYYDiagnosisSliceDefCommand,SYYDiagnosisSliceDefMutationResponse>
 {
 public async Task<SYYDiagnosisSliceDefMutationResponse> Handle(UpdateSYYDiagnosisSliceDefCommand r,CancellationToken ct){
 var item=await SYYDiagnosisSliceDefRules.LoadAsync(db,r.Id,r.Version,ct);
 
 
 item.Name=r.Name.Trim(); // Eski int kod, bağlı tanıların referansını bozmamak için değişmez.
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
