using FluentValidation;

namespace KLMN.Application.PurchaseNumbers.Commands.ChangePurchaseNumberDefStatus;

public sealed class ChangePurchaseNumberDefStatusCommandValidator : AbstractValidator<ChangePurchaseNumberDefStatusCommand>
{
    public ChangePurchaseNumberDefStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
