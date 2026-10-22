using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.SYYDiagnosisTypes.Commands.Common;
 using KLMN.Application.SYYDiagnosisTypes.Responses;
 
 using MediatR;
 namespace KLMN.Application.SYYDiagnosisTypes.Commands.UpdateSYYDiagnosisTypeDef;
 /// <summary>SYY Tanı Türü Tanımları Update iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class UpdateSYYDiagnosisTypeDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<UpdateSYYDiagnosisTypeDefCommand,SYYDiagnosisTypeDefMutationResponse>
 {
 public async Task<SYYDiagnosisTypeDefMutationResponse> Handle(UpdateSYYDiagnosisTypeDefCommand r,CancellationToken ct){
 var item=await SYYDiagnosisTypeDefRules.LoadAsync(db,r.Id,r.Version,ct);
 
 
 item.Name=r.Name.Trim(); // Eski int kod, bağlı tanıların referansını bozmamak için değişmez.
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
