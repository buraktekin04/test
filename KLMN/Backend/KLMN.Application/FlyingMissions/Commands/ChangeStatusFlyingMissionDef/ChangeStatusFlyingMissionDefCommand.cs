using MediatR;
 using KLMN.Application.FlyingMissions.Responses;
 namespace KLMN.Application.FlyingMissions.Commands.ChangeStatusFlyingMissionDef;
 /// <summary>Uçucu Görev Tanımları: ChangeStatus komutu.</summary>
 public sealed record ChangeStatusFlyingMissionDefCommand(Guid Id,uint Version,bool IsActive):IRequest<FlyingMissionDefMutationResponse>;
