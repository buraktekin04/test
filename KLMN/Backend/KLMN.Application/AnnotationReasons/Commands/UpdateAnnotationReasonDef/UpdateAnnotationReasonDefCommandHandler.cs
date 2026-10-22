using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.AnnotationReasons.Commands.Common;
 using KLMN.Application.AnnotationReasons.Responses;
 
 using MediatR;
 namespace KLMN.Application.AnnotationReasons.Commands.UpdateAnnotationReasonDef;
 /// <summary>Sevk Sebebi Tanımları Update iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class UpdateAnnotationReasonDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<UpdateAnnotationReasonDefCommand,AnnotationReasonDefMutationResponse>
 {
 public async Task<AnnotationReasonDefMutationResponse> Handle(UpdateAnnotationReasonDefCommand r,CancellationToken ct){
 var item=await AnnotationReasonDefRules.LoadAsync(db,r.Id,r.Version,ct);
 await AnnotationReasonDefRules.EnsureUniqueAsync(db,r.Code,item.Id,ct);
 
 item.Code=r.Code.Trim().ToUpperInvariant();item.Name=r.Name.Trim();item.ValidFrom=r.ValidFrom;item.ValidTo=r.ValidTo;
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
