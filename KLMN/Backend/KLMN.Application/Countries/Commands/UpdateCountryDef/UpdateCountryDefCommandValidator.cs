using FluentValidation;

namespace KLMN.Application.Countries.Commands.UpdateCountryDef;
/// <summary>Kısa kod (ShortCode) ve kayıt sürümü dahil ülke güncelleme doğrulaması.</summary>
public sealed class UpdateCountryDefCommandValidator : AbstractValidator<UpdateCountryDefCommand>
{
    public UpdateCountryDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ExternalCode).MaximumLength(100);
        RuleFor(x => x.ShortCode).MaximumLength(10)
            .Matches("^[A-Za-z]{2,10}$")
            .When(x => !string.IsNullOrWhiteSpace(x.ShortCode));

    }
}
