using FluentValidation;

namespace KLMN.Application.SYYClassifications.Commands.ChangeStatusSYYClassificationDef;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi: ChangeStatus doğrulama.</summary>
public sealed class ChangeStatusSYYClassificationDefCommandValidator:AbstractValidator<ChangeStatusSYYClassificationDefCommand>
{
 public ChangeStatusSYYClassificationDefCommandValidator()
 {
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
}
