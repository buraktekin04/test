using FluentValidation;
 using KLMN.Domain.Common;
 namespace KLMN.Application.FlyingMissions.Commands.DeleteFlyingMissionDef;
 /// <summary>Uçucu Görev Tanımları Delete doğrulaması.</summary>
 public sealed class DeleteFlyingMissionDefCommandValidator:AbstractValidator<DeleteFlyingMissionDefCommand>
 {
 public DeleteFlyingMissionDefCommandValidator(){
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
 }
