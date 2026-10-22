using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.AnnotationReasons.Commands.Common;
 
 
 using MediatR;
 namespace KLMN.Application.AnnotationReasons.Commands.DeleteAnnotationReasonDef;
 /// <summary>Sevk Sebebi Tanımları Delete iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class DeleteAnnotationReasonDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<DeleteAnnotationReasonDefCommand>
 {
 public async Task Handle(DeleteAnnotationReasonDefCommand r,CancellationToken ct){
 var item=await AnnotationReasonDefRules.LoadAsync(db,r.Id,r.Version,ct);
 
 db.AnnotationReasonDefs.Remove(item);await db.SaveChangesAsync(ct);
 }
 }
