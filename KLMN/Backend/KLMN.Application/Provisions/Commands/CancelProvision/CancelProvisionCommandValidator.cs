using FluentValidation;

namespace KLMN.Application.Provisions.Commands.CancelProvision;

public sealed class CancelProvisionCommandValidator : AbstractValidator<CancelProvisionCommand>
{
    public CancelProvisionCommandValidator()
    {
        RuleFor(x=>x.Id).NotEmpty();
        RuleFor(x=>x.Version).GreaterThan(0u);
        RuleFor(x=>x.Reason).NotEmpty().MaximumLength(1000);
    }
}
