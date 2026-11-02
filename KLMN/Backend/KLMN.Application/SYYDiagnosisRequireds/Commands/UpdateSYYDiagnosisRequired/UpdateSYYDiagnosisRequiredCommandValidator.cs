using FluentValidation;
using KLMN.Application.SYYDiagnosisRequireds.Commands.Common;
namespace KLMN.Application.SYYDiagnosisRequireds.Commands.UpdateSYYDiagnosisRequired;
public sealed class UpdateSYYDiagnosisRequiredCommandValidator:AbstractValidator<UpdateSYYDiagnosisRequiredCommand>
{
 public UpdateSYYDiagnosisRequiredCommandValidator()
 {
  RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
  
 RuleFor(x=>x.SYYDiagnosisDefId).NotEmpty();
 RuleFor(x=>x).Must(x=>SYYDiagnosisRequiredRules.HasExactlyOneCondition(x.MustAll,x.MustOneOf))
 .WithMessage("Kural koşulunda Tüm Maddeler Gerekli veya En Az Biri Gerekli seçeneklerinden tam olarak biri seçilmelidir.");
 RuleFor(x=>x.Descriptions).NotNull().Must(x=>x is { Count: >= 1 and <= 50 })
 .WithMessage("1 ile 50 arası madde yazılmalıdır.");
 RuleForEach(x=>x.Descriptions).NotEmpty().MaximumLength(1500);
 }
}
