using FluentValidation;
namespace KLMN.Application.SYYDiagnosisRequireds.Commands.UpdateSYYDiagnosisRequired;
public sealed class UpdateSYYDiagnosisRequiredCommandValidator:AbstractValidator<UpdateSYYDiagnosisRequiredCommand>
{
 public UpdateSYYDiagnosisRequiredCommandValidator()
 {
  RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
  
 RuleFor(x=>x.SYYDiagnosisDefId).NotEmpty();
 RuleFor(x=>x).Must(x=>!(x.MustAll==true&&x.MustOneOf==true))
 .WithMessage("Tüm maddeler ve en az biri kuralları birlikte seçilemez.");
 RuleFor(x=>x.Descriptions).NotNull().Must(x=>x is { Count: >= 1 and <= 50 })
 .WithMessage("1 ile 50 arası madde yazılmalıdır.");
 RuleForEach(x=>x.Descriptions).NotEmpty().MaximumLength(1500);
 }
}
