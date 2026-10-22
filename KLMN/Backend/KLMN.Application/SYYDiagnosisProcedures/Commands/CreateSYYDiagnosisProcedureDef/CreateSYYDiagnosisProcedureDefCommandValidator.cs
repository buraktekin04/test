using FluentValidation;

namespace KLMN.Application.SYYDiagnosisProcedures.Commands.CreateSYYDiagnosisProcedureDef;
/// <summary>SYY Gereken Tetkikler: Create doğrulama.</summary>
public sealed class CreateSYYDiagnosisProcedureDefCommandValidator:AbstractValidator<CreateSYYDiagnosisProcedureDefCommand>
{
 public CreateSYYDiagnosisProcedureDefCommandValidator()
 {
 
 RuleFor(x=>x.SYYDiagnosisDefId).NotEmpty();
RuleFor(x=>x.ProcedureDefId).NotEmpty();
 RuleFor(x=>x.StartingAge).InclusiveBetween(0,130);
 RuleFor(x=>x.EndingAge).InclusiveBetween(0,130);
 RuleFor(x=>x.Gender).InclusiveBetween(0,20);
 RuleFor(x=>x).Must(x=>x.StartingAge<=x.EndingAge)
 .WithMessage("Başlangıç yaşı bitiş yaşını aşamaz.");
 }
}
