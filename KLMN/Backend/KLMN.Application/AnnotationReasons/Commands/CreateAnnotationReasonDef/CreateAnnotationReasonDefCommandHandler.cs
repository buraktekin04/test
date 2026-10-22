using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.AnnotationReasons.Commands.Common;
 using KLMN.Application.AnnotationReasons.Responses;
 using KLMN.Domain.Entities.Admissions;
 using MediatR;
 namespace KLMN.Application.AnnotationReasons.Commands.CreateAnnotationReasonDef;
 /// <summary>Sevk Sebebi Tanımları Create iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class CreateAnnotationReasonDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<CreateAnnotationReasonDefCommand,AnnotationReasonDefMutationResponse>
 {
 public async Task<AnnotationReasonDefMutationResponse> Handle(CreateAnnotationReasonDefCommand r,CancellationToken ct){
 await AnnotationReasonDefRules.EnsureUniqueAsync(db,r.Code,null,ct);
 
 var item=new AnnotationReasonDef{Code=r.Code.Trim().ToUpperInvariant(),Name=r.Name.Trim(),ValidFrom=r.ValidFrom,ValidTo=r.ValidTo};
 await db.AnnotationReasonDefs.AddAsync(item,ct);await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
