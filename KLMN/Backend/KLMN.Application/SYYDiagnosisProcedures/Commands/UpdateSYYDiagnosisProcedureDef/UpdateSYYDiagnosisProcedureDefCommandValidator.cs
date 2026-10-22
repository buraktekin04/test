using FluentValidation;

namespace KLMN.Application.SYYDiagnosisProcedures.Commands.UpdateSYYDiagnosisProcedureDef;
/// <summary>SYY Gereken Tetkikler: Update doğrulama.</summary>
public sealed class UpdateSYYDiagnosisProcedureDefCommandValidator:AbstractValidator<UpdateSYYDiagnosisProcedureDefCommand>
{
 public UpdateSYYDiagnosisProcedureDefCommandValidator()
 {
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 RuleFor(x=>x.SYYDiagnosisDefId).NotEmpty();
RuleFor(x=>x.ProcedureDefId).NotEmpty();
 RuleFor(x=>x.StartingAge).InclusiveBetween(0,130);
 RuleFor(x=>x.EndingAge).InclusiveBetween(0,130);
 RuleFor(x=>x.Gender).InclusiveBetween(0,20);
 RuleFor(x=>x).Must(x=>x.StartingAge<=x.EndingAge)
 .WithMessage("Başlangıç yaşı bitiş yaşını aşamaz.");
 }
}
