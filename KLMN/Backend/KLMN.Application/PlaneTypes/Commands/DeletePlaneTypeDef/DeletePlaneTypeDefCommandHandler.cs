using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.PlaneTypes.Commands.Common;
 
 
 using MediatR;
 namespace KLMN.Application.PlaneTypes.Commands.DeletePlaneTypeDef;
 /// <summary>Uçuşa Uçak Tipi Tanımları Delete iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class DeletePlaneTypeDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<DeletePlaneTypeDefCommand>
 {
 public async Task Handle(DeletePlaneTypeDefCommand r,CancellationToken ct){
 var item=await PlaneTypeDefRules.LoadAsync(db,r.Id,r.Version,ct);
 
 db.PlaneTypeDefs.Remove(item);await db.SaveChangesAsync(ct);
 }
 }
