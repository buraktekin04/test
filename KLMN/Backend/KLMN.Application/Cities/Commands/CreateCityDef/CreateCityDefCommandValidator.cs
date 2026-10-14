using FluentValidation;
using KLMN.Domain.Common;
namespace KLMN.Application.Cities.Commands.CreateCityDef;
public sealed class CreateCityDefCommandValidator : AbstractValidator<CreateCityDefCommand>
{
    public CreateCityDefCommandValidator()
    {

        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ExternalCode).MaximumLength(100);
        RuleFor(x => x.CountryId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x.MernisCode).GreaterThanOrEqualTo(0).When(x => x.MernisCode.HasValue);
        RuleFor(x => x).Must(x => ValidityPeriod.HasValidBounds(x.ValidFrom, x.ValidTo))
            .WithMessage("Geçerli bitiş başlangıçtan sonra olmalıdır.").WithName("ValidTo");
        RuleFor(x => x.ValidFrom).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc);
        RuleFor(x => x.ValidTo).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc);
    }
}
