using MediatR;
 using KLMN.Application.FlyingMissions.Responses;
 namespace KLMN.Application.FlyingMissions.Commands.UpdateFlyingMissionDef;
 /// <summary>Uçucu Görev Tanımları: Update komutu.</summary>
 public sealed record UpdateFlyingMissionDefCommand(Guid Id,uint Version,string Code,string Name,DateTime? ValidFrom,DateTime? ValidTo):IRequest<FlyingMissionDefMutationResponse>;
