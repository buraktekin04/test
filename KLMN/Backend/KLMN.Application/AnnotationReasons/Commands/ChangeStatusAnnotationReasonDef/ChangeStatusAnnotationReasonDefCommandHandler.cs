using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.AnnotationReasons.Commands.Common;
 using KLMN.Application.AnnotationReasons.Responses;
 
 using MediatR;
 namespace KLMN.Application.AnnotationReasons.Commands.ChangeStatusAnnotationReasonDef;
 /// <summary>Zeyil Sebebi Tanımları ChangeStatus iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class ChangeStatusAnnotationReasonDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<ChangeStatusAnnotationReasonDefCommand,AnnotationReasonDefMutationResponse>
 {
 public async Task<AnnotationReasonDefMutationResponse> Handle(ChangeStatusAnnotationReasonDefCommand r,CancellationToken ct){
 var item=await AnnotationReasonDefRules.LoadAsync(db,r.Id,r.Version,ct);
 
 item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
