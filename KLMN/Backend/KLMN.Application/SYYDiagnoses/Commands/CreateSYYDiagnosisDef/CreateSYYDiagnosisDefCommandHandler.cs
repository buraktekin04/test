using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.SYYDiagnoses.Commands.Common;
 using KLMN.Application.SYYDiagnoses.Responses;
 using KLMN.Domain.Entities.Admissions;
 using MediatR;
 namespace KLMN.Application.SYYDiagnoses.Commands.CreateSYYDiagnosisDef;
 /// <summary>SYY Tanı Listesi (M/D/F) Create iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class CreateSYYDiagnosisDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<CreateSYYDiagnosisDefCommand,SYYDiagnosisDefMutationResponse>
 {
 public async Task<SYYDiagnosisDefMutationResponse> Handle(CreateSYYDiagnosisDefCommand r,CancellationToken ct){
 await SYYDiagnosisDefRules.EnsureUniqueAsync(db,r.Code,null,ct);
 await SYYDiagnosisDefRules.EnsureRelationsAsync(db,Guid.Empty,r.SYYDiagnosisType,r.AttachedRecNo,r.Slice,
 null,null,null,ct);
 var typeId=await SYYDiagnosisDefRules.ResolveTypeIdAsync(db,r.SYYDiagnosisType,ct);
 var sliceId=await SYYDiagnosisDefRules.ResolveSliceIdAsync(db,r.Slice,ct);
 var item=new SYYDiagnosisDef{Code=r.Code.Trim().ToUpperInvariant(),Name=r.Name.Trim(),SYYDiagnosisType=r.SYYDiagnosisType,SYYDiagnosisTypeDefId=typeId,
 AttachedRecNo=r.AttachedRecNo,Eligible=r.Eligible,Slice=r.Slice,SYYDiagnosisSliceDefId=sliceId,ValidFrom=r.ValidFrom,ValidTo=r.ValidTo};
 await db.SYYDiagnosisDefs.AddAsync(item,ct);await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
