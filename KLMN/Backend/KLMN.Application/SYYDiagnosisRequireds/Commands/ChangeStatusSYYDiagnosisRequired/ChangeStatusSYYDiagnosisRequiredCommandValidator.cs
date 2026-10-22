using FluentValidation;
namespace KLMN.Application.SYYDiagnosisRequireds.Commands.ChangeStatusSYYDiagnosisRequired;
public sealed class ChangeStatusSYYDiagnosisRequiredCommandValidator:AbstractValidator<ChangeStatusSYYDiagnosisRequiredCommand>
{
 public ChangeStatusSYYDiagnosisRequiredCommandValidator()
 {
  RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
  
 }
}
