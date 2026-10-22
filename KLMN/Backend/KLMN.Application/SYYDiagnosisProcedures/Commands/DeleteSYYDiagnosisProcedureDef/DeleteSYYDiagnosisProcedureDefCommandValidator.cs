using FluentValidation;

namespace KLMN.Application.SYYDiagnosisProcedures.Commands.DeleteSYYDiagnosisProcedureDef;
/// <summary>SYY Gereken Tetkikler: Delete doğrulama.</summary>
public sealed class DeleteSYYDiagnosisProcedureDefCommandValidator:AbstractValidator<DeleteSYYDiagnosisProcedureDefCommand>
{
 public DeleteSYYDiagnosisProcedureDefCommandValidator()
 {
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
}
