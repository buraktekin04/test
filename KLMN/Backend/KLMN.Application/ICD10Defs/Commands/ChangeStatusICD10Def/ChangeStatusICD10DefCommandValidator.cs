using FluentValidation;
namespace KLMN.Application.ICD10Defs.Commands.ChangeStatusICD10Def;
/// <summary>ICD10 Tanımları: ChangeStatus alan doğrulamaları.</summary>
public sealed class ChangeStatusICD10DefCommandValidator:AbstractValidator<ChangeStatusICD10DefCommand>
{
 public ChangeStatusICD10DefCommandValidator()
 {
 RuleFor(x=>x.Id).NotEmpty(); RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
}
