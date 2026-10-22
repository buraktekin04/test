using FluentValidation;
 using KLMN.Domain.Common;
 namespace KLMN.Application.PlaneTypes.Commands.ChangeStatusPlaneTypeDef;
 /// <summary>Uçuşa Uçak Tipi Tanımları ChangeStatus doğrulaması.</summary>
 public sealed class ChangeStatusPlaneTypeDefCommandValidator:AbstractValidator<ChangeStatusPlaneTypeDefCommand>
 {
 public ChangeStatusPlaneTypeDefCommandValidator(){
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
 }
