using FluentValidation;
namespace KLMN.Application.Cities.Commands.DeleteCityDef;
public sealed class DeleteCityDefCommandValidator : AbstractValidator<DeleteCityDefCommand>
{
    public DeleteCityDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
