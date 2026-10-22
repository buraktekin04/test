using FluentValidation;
namespace KLMN.Application.SYYDiagnosisRequireds.Commands.DeleteSYYDiagnosisRequired;
public sealed class DeleteSYYDiagnosisRequiredCommandValidator:AbstractValidator<DeleteSYYDiagnosisRequiredCommand>
{
 public DeleteSYYDiagnosisRequiredCommandValidator()
 {
  RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
  
 }
}
