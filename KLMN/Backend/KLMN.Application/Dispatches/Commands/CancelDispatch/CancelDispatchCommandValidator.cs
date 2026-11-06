using FluentValidation;

namespace KLMN.Application.Dispatches.Commands.CancelDispatch;

/// <summary>Sevk iptali gerekçesini zorunlu tutar, diğer sevk açıklamasını değiştirmez.</summary>
public sealed class CancelDispatchCommandValidator : AbstractValidator<CancelDispatchCommand>
{
    public CancelDispatchCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
