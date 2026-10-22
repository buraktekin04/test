using FluentValidation;

namespace KLMN.Application.SYYDiagnosisBranches.Commands.DeleteSYYDiagnosisBranchDef;
/// <summary>SYY Gereken Branşlar: Delete doğrulama.</summary>
public sealed class DeleteSYYDiagnosisBranchDefCommandValidator:AbstractValidator<DeleteSYYDiagnosisBranchDefCommand>
{
 public DeleteSYYDiagnosisBranchDefCommandValidator()
 {
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
}
