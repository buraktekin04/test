using FluentValidation;
 using KLMN.Domain.Common;
 namespace KLMN.Application.PlaneTypes.Commands.DeletePlaneTypeDef;
 /// <summary>Uçuşa Uçak Tipi Tanımları Delete doğrulaması.</summary>
 public sealed class DeletePlaneTypeDefCommandValidator:AbstractValidator<DeletePlaneTypeDefCommand>
 {
 public DeletePlaneTypeDefCommandValidator(){
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
 }
