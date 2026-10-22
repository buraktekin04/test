using FluentValidation;

namespace KLMN.Application.SYYDiagnosisBranches.Commands.UpdateSYYDiagnosisBranchDef;
/// <summary>SYY Gereken Branşlar: Update doğrulama.</summary>
public sealed class UpdateSYYDiagnosisBranchDefCommandValidator:AbstractValidator<UpdateSYYDiagnosisBranchDefCommand>
{
 public UpdateSYYDiagnosisBranchDefCommandValidator()
 {
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 RuleFor(x=>x.SYYDiagnosisDefId).NotEmpty();
RuleFor(x=>x.BranchDefId).NotEmpty();
 RuleFor(x=>x.StartingAge).InclusiveBetween(0,130);
 RuleFor(x=>x.EndingAge).InclusiveBetween(0,130);
 RuleFor(x=>x.Gender).InclusiveBetween(0,20);
 RuleFor(x=>x).Must(x=>x.StartingAge<=x.EndingAge)
 .WithMessage("Başlangıç yaşı bitiş yaşını aşamaz.");
 }
}
