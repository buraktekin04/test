using FluentValidation;
namespace KLMN.Application.Countries.Commands.ChangeStatusCountryDef;
public sealed class ChangeStatusCountryDefCommandValidator : AbstractValidator<ChangeStatusCountryDefCommand>
{
    public ChangeStatusCountryDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
