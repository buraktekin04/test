using FluentValidation;
namespace KLMN.Application.Cities.Commands.ChangeStatusCityDef;
public sealed class ChangeStatusCityDefCommandValidator : AbstractValidator<ChangeStatusCityDefCommand>
{
    public ChangeStatusCityDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
