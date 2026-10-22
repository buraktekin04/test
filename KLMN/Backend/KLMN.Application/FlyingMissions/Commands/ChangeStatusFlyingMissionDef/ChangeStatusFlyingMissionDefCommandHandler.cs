using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.FlyingMissions.Commands.Common;
 using KLMN.Application.FlyingMissions.Responses;
 
 using MediatR;
 namespace KLMN.Application.FlyingMissions.Commands.ChangeStatusFlyingMissionDef;
 /// <summary>Uçucu Görev Tanımları ChangeStatus iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class ChangeStatusFlyingMissionDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<ChangeStatusFlyingMissionDefCommand,FlyingMissionDefMutationResponse>
 {
 public async Task<FlyingMissionDefMutationResponse> Handle(ChangeStatusFlyingMissionDefCommand r,CancellationToken ct){
 var item=await FlyingMissionDefRules.LoadAsync(db,r.Id,r.Version,ct);
 
 item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
