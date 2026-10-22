using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.FlyingMissions.Commands.Common;
 using KLMN.Application.FlyingMissions.Responses;
 
 using MediatR;
 namespace KLMN.Application.FlyingMissions.Commands.UpdateFlyingMissionDef;
 /// <summary>Uçucu Görev Tanımları Update iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class UpdateFlyingMissionDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<UpdateFlyingMissionDefCommand,FlyingMissionDefMutationResponse>
 {
 public async Task<FlyingMissionDefMutationResponse> Handle(UpdateFlyingMissionDefCommand r,CancellationToken ct){
 var item=await FlyingMissionDefRules.LoadAsync(db,r.Id,r.Version,ct);
 await FlyingMissionDefRules.EnsureUniqueAsync(db,r.Code,item.Id,ct);
 
 item.Code=r.Code.Trim().ToUpperInvariant();item.Name=r.Name.Trim();item.ValidFrom=r.ValidFrom;item.ValidTo=r.ValidTo;
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
