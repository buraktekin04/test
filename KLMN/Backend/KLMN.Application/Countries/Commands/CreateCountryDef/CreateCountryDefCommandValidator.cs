using FluentValidation;

namespace KLMN.Application.Countries.Commands.CreateCountryDef;
public sealed class CreateCountryDefCommandValidator : AbstractValidator<CreateCountryDefCommand>
{
    public CreateCountryDefCommandValidator()
    {

        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ExternalCode).MaximumLength(100);

    }
}
