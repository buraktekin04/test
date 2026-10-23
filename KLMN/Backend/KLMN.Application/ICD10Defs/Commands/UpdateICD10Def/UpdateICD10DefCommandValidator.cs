using FluentValidation;
namespace KLMN.Application.ICD10Defs.Commands.UpdateICD10Def;
/// <summary>ICD10 Tanımları: Update alan doğrulamaları.</summary>
public sealed class UpdateICD10DefCommandValidator:AbstractValidator<UpdateICD10DefCommand>
{
 public UpdateICD10DefCommandValidator()
 {
 RuleFor(x=>x.Id).NotEmpty(); RuleFor(x=>x.Version).GreaterThan(0u);
 RuleFor(x=>x.Code).NotEmpty().MaximumLength(32);
  RuleFor(x=>x.Name).NotEmpty().MaximumLength(500);
 }
}
