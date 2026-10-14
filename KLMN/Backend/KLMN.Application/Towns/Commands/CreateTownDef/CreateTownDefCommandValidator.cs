using FluentValidation;
namespace KLMN.Application.Towns.Commands.CreateTownDef;
/// <summary>İlçe tanımı işlem veya doğrulama sözleşmesi; ilgili CQRS katmanında kullanılır.</summary>
public sealed class CreateTownDefCommandValidator : AbstractValidator<CreateTownDefCommand>
{
    public CreateTownDefCommandValidator()
    {

        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ExternalCode).MaximumLength(100);
        RuleFor(x => x.CityId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x.MernisCode).GreaterThanOrEqualTo(0).When(x => x.MernisCode.HasValue);
    }
}
