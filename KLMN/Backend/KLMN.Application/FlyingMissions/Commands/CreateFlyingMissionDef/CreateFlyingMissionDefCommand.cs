using MediatR;
 using KLMN.Application.FlyingMissions.Responses;
 namespace KLMN.Application.FlyingMissions.Commands.CreateFlyingMissionDef;
 /// <summary>Uçucu Görev Tanımları: Create komutu.</summary>
 public sealed record CreateFlyingMissionDefCommand(string Code,string Name,DateTime? ValidFrom,DateTime? ValidTo):IRequest<FlyingMissionDefMutationResponse>;
