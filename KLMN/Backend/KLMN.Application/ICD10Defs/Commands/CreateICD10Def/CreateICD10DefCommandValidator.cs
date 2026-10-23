using FluentValidation;
namespace KLMN.Application.ICD10Defs.Commands.CreateICD10Def;
/// <summary>ICD10 Tanımları: Create alan doğrulamaları.</summary>
public sealed class CreateICD10DefCommandValidator:AbstractValidator<CreateICD10DefCommand>
{
 public CreateICD10DefCommandValidator()
 {
 
 RuleFor(x=>x.Code).NotEmpty().MaximumLength(32);
  RuleFor(x=>x.Name).NotEmpty().MaximumLength(500);
 }
}
