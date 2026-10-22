using MediatR;
 
 namespace KLMN.Application.FlyingMissions.Commands.DeleteFlyingMissionDef;
 /// <summary>Uçucu Görev Tanımları: Delete komutu.</summary>
 public sealed record DeleteFlyingMissionDefCommand(Guid Id,uint Version):IRequest;
