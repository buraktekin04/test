using FluentValidation;

namespace KLMN.Application.SYYDiagnosisProcedures.Commands.ChangeStatusSYYDiagnosisProcedureDef;
/// <summary>SYY Gereken Tetkikler: ChangeStatus doğrulama.</summary>
public sealed class ChangeStatusSYYDiagnosisProcedureDefCommandValidator:AbstractValidator<ChangeStatusSYYDiagnosisProcedureDefCommand>
{
 public ChangeStatusSYYDiagnosisProcedureDefCommandValidator()
 {
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
}
