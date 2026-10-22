using FluentValidation;

namespace KLMN.Application.SYYClassifications.Commands.DeleteSYYClassificationDef;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi: Delete doğrulama.</summary>
public sealed class DeleteSYYClassificationDefCommandValidator:AbstractValidator<DeleteSYYClassificationDefCommand>
{
 public DeleteSYYClassificationDefCommandValidator()
 {
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
}
