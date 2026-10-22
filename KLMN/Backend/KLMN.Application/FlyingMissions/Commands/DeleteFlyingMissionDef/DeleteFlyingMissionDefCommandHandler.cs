using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.FlyingMissions.Commands.Common;
 
 
 using MediatR;
 namespace KLMN.Application.FlyingMissions.Commands.DeleteFlyingMissionDef;
 /// <summary>Uçucu Görev Tanımları Delete iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class DeleteFlyingMissionDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<DeleteFlyingMissionDefCommand>
 {
 public async Task Handle(DeleteFlyingMissionDefCommand r,CancellationToken ct){
 var item=await FlyingMissionDefRules.LoadAsync(db,r.Id,r.Version,ct);
 
 db.FlyingMissionDefs.Remove(item);await db.SaveChangesAsync(ct);
 }
 }
