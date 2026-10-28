using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.SYYDiagnosisTypes.Commands.Common;
 using KLMN.Application.SYYDiagnosisTypes.Responses;
 
 using MediatR;
 namespace KLMN.Application.SYYDiagnosisTypes.Commands.ChangeStatusSYYDiagnosisTypeDef;
 /// <summary>SYY Tanı Türü Tanımları ChangeStatus iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class ChangeStatusSYYDiagnosisTypeDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<ChangeStatusSYYDiagnosisTypeDefCommand,SYYDiagnosisTypeDefMutationResponse>
 {
 public async Task<SYYDiagnosisTypeDefMutationResponse> Handle(ChangeStatusSYYDiagnosisTypeDefCommand r,CancellationToken ct){
 var item=await SYYDiagnosisTypeDefRules.LoadAsync(db,r.Id,r.Version,ct);
 if(!r.IsActive)await SYYDiagnosisTypeDefRules.EnsureUnusedAsync(db,item.Id,ct);
 item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
