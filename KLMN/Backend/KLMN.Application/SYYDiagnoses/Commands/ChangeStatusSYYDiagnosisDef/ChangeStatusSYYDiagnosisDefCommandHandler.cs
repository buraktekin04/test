using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.SYYDiagnoses.Commands.Common;
 using KLMN.Application.SYYDiagnoses.Responses;
 
 using MediatR;
 namespace KLMN.Application.SYYDiagnoses.Commands.ChangeStatusSYYDiagnosisDef;
 /// <summary>SYY Tanı Listesi (M/D/F) ChangeStatus iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class ChangeStatusSYYDiagnosisDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<ChangeStatusSYYDiagnosisDefCommand,SYYDiagnosisDefMutationResponse>
 {
 public async Task<SYYDiagnosisDefMutationResponse> Handle(ChangeStatusSYYDiagnosisDefCommand r,CancellationToken ct){
 var item=await SYYDiagnosisDefRules.LoadAsync(db,r.Id,r.Version,ct);
 if(!r.IsActive)await SYYDiagnosisDefRules.EnsureNoChildrenAsync(db,item.Id,true,ct);
 if(r.IsActive)await SYYDiagnosisDefRules.EnsureRelationsAsync(db,item.Id,item.SYYDiagnosisType,item.AttachedRecNo,item.Slice,
 null,null,null,ct);
 item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
