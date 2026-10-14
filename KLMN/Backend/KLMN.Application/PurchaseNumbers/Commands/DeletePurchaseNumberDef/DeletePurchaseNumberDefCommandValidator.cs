using FluentValidation;

namespace KLMN.Application.PurchaseNumbers.Commands.DeletePurchaseNumberDef;

public sealed class DeletePurchaseNumberDefCommandValidator : AbstractValidator<DeletePurchaseNumberDefCommand>
{
    public DeletePurchaseNumberDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
