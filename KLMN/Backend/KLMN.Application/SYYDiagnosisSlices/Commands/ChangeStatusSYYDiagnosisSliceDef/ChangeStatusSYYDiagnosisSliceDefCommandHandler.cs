using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.SYYDiagnosisSlices.Commands.Common;
 using KLMN.Application.SYYDiagnosisSlices.Responses;
 
 using MediatR;
 namespace KLMN.Application.SYYDiagnosisSlices.Commands.ChangeStatusSYYDiagnosisSliceDef;
 /// <summary>SYY Dilim Tanımları ChangeStatus iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class ChangeStatusSYYDiagnosisSliceDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<ChangeStatusSYYDiagnosisSliceDefCommand,SYYDiagnosisSliceDefMutationResponse>
 {
 public async Task<SYYDiagnosisSliceDefMutationResponse> Handle(ChangeStatusSYYDiagnosisSliceDefCommand r,CancellationToken ct){
 var item=await SYYDiagnosisSliceDefRules.LoadAsync(db,r.Id,r.Version,ct);
 if(!r.IsActive)await SYYDiagnosisSliceDefRules.EnsureUnusedAsync(db,item.Code,ct);
 item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
