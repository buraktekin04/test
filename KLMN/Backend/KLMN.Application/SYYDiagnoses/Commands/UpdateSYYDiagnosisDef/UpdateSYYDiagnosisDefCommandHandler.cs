using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.SYYDiagnoses.Commands.Common;
 using KLMN.Application.SYYDiagnoses.Responses;
 
 using MediatR;
 namespace KLMN.Application.SYYDiagnoses.Commands.UpdateSYYDiagnosisDef;
 /// <summary>SYY Tanı Listesi (M/D/F) Update iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class UpdateSYYDiagnosisDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<UpdateSYYDiagnosisDefCommand,SYYDiagnosisDefMutationResponse>
 {
 public async Task<SYYDiagnosisDefMutationResponse> Handle(UpdateSYYDiagnosisDefCommand r,CancellationToken ct){
 var item=await SYYDiagnosisDefRules.LoadAsync(db,r.Id,r.Version,ct);
 await SYYDiagnosisDefRules.EnsureUniqueAsync(db,r.Code,item.Id,ct);
 await SYYDiagnosisDefRules.EnsureRelationsAsync(db,item.Id,r.SYYDiagnosisType,r.AttachedRecNo,r.Slice,
 item.AttachedRecNo,item.SYYDiagnosisType,item.Slice,ct);
 item.Code=r.Code.Trim().ToUpperInvariant();item.Name=r.Name.Trim();
 item.SYYDiagnosisType=r.SYYDiagnosisType;item.AttachedRecNo=r.AttachedRecNo;item.Eligible=r.Eligible;
 item.Slice=r.Slice;item.ValidFrom=r.ValidFrom;item.ValidTo=r.ValidTo;
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
