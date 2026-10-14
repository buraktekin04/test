using FluentValidation;
namespace KLMN.Application.Countries.Commands.DeleteCountryDef;
public sealed class DeleteCountryDefCommandValidator : AbstractValidator<DeleteCountryDefCommand>
{
    public DeleteCountryDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
