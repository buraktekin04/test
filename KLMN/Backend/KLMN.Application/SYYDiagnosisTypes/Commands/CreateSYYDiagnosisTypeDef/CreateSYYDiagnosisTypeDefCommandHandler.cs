using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.SYYDiagnosisTypes.Commands.Common;
 using KLMN.Application.SYYDiagnosisTypes.Responses;
 using KLMN.Domain.Entities.Admissions;
 using MediatR;
 namespace KLMN.Application.SYYDiagnosisTypes.Commands.CreateSYYDiagnosisTypeDef;
 /// <summary>SYY Tanı Türü Tanımları Create iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class CreateSYYDiagnosisTypeDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<CreateSYYDiagnosisTypeDefCommand,SYYDiagnosisTypeDefMutationResponse>
 {
 public async Task<SYYDiagnosisTypeDefMutationResponse> Handle(CreateSYYDiagnosisTypeDefCommand r,CancellationToken ct){
 await SYYDiagnosisTypeDefRules.EnsureUniqueAsync(db,r.Code,null,ct);
 
 var item=new SYYDiagnosisTypeDef{Code=r.Code,Name=r.Name.Trim()};
 await db.SYYDiagnosisTypeDefs.AddAsync(item,ct);await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
