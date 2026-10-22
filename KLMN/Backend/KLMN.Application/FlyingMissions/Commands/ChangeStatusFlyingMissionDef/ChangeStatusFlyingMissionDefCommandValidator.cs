using FluentValidation;
 using KLMN.Domain.Common;
 namespace KLMN.Application.FlyingMissions.Commands.ChangeStatusFlyingMissionDef;
 /// <summary>Uçucu Görev Tanımları ChangeStatus doğrulaması.</summary>
 public sealed class ChangeStatusFlyingMissionDefCommandValidator:AbstractValidator<ChangeStatusFlyingMissionDefCommand>
 {
 public ChangeStatusFlyingMissionDefCommandValidator(){
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
 }
