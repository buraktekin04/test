using FluentValidation;

namespace KLMN.Application.SYYDiagnosisBranches.Commands.CreateSYYDiagnosisBranchDef;
/// <summary>SYY Gereken Branşlar: Create doğrulama.</summary>
public sealed class CreateSYYDiagnosisBranchDefCommandValidator:AbstractValidator<CreateSYYDiagnosisBranchDefCommand>
{
 public CreateSYYDiagnosisBranchDefCommandValidator()
 {
 
 RuleFor(x=>x.SYYDiagnosisDefId).NotEmpty();
RuleFor(x=>x.BranchDefId).NotEmpty();
 RuleFor(x=>x.StartingAge).InclusiveBetween(0,130);
 RuleFor(x=>x.EndingAge).InclusiveBetween(0,130);
 RuleFor(x=>x.Gender).InclusiveBetween(0,20);
 RuleFor(x=>x).Must(x=>x.StartingAge<=x.EndingAge)
 .WithMessage("Başlangıç yaşı bitiş yaşını aşamaz.");
 }
}
