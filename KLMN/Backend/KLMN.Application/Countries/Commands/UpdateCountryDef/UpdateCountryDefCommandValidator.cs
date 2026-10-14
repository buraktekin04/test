using FluentValidation;

namespace KLMN.Application.Countries.Commands.UpdateCountryDef;
public sealed class UpdateCountryDefCommandValidator : AbstractValidator<UpdateCountryDefCommand>
{
    public UpdateCountryDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ExternalCode).MaximumLength(100);

    }
}
