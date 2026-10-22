using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.SYYDiagnosisSlices.Commands.Common;
 using KLMN.Application.SYYDiagnosisSlices.Responses;
 using KLMN.Domain.Entities.Admissions;
 using MediatR;
 namespace KLMN.Application.SYYDiagnosisSlices.Commands.CreateSYYDiagnosisSliceDef;
 /// <summary>SYY Dilim Tanımları Create iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class CreateSYYDiagnosisSliceDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<CreateSYYDiagnosisSliceDefCommand,SYYDiagnosisSliceDefMutationResponse>
 {
 public async Task<SYYDiagnosisSliceDefMutationResponse> Handle(CreateSYYDiagnosisSliceDefCommand r,CancellationToken ct){
 await SYYDiagnosisSliceDefRules.EnsureUniqueAsync(db,r.Code,null,ct);
 
 var item=new SYYDiagnosisSliceDef{Code=r.Code,Name=r.Name.Trim()};
 await db.SYYDiagnosisSliceDefs.AddAsync(item,ct);await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
