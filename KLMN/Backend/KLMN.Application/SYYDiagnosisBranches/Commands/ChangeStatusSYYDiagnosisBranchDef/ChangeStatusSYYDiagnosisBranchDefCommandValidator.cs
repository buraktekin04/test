using FluentValidation;

namespace KLMN.Application.SYYDiagnosisBranches.Commands.ChangeStatusSYYDiagnosisBranchDef;
/// <summary>SYY Gereken Branşlar: ChangeStatus doğrulama.</summary>
public sealed class ChangeStatusSYYDiagnosisBranchDefCommandValidator:AbstractValidator<ChangeStatusSYYDiagnosisBranchDefCommand>
{
 public ChangeStatusSYYDiagnosisBranchDefCommandValidator()
 {
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
}
