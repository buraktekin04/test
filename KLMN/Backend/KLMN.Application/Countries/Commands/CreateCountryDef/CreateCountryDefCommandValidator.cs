using FluentValidation;

namespace KLMN.Application.Countries.Commands.CreateCountryDef;
/// <summary>CountryDef kısa kodunun harici entegrasyon kodundan ayrı biçimini doğrular.</summary>
public sealed class CreateCountryDefCommandValidator : AbstractValidator<CreateCountryDefCommand>
{
    public CreateCountryDefCommandValidator()
    {

        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ExternalCode).MaximumLength(100);
        // Kısa ülke kodu opsiyoneldir; doluysa 2–10 Latin harften oluşur.
        RuleFor(x => x.ShortCode).MaximumLength(10)
            .Matches("^[A-Za-z]{2,10}$")
            .When(x => !string.IsNullOrWhiteSpace(x.ShortCode));

    }
}
