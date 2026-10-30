using FluentValidation;
namespace KLMN.Application.Dispatches.Commands.CancelDispatch;
/// <summary>İptal için geçerli kayıt ve sürüm bilgisi zorunludur.</summary>
public sealed class CancelDispatchCommandValidator : AbstractValidator<CancelDispatchCommand>
{
    public CancelDispatchCommandValidator()
    {
        RuleFor(x=>x.Id).NotEmpty();
        RuleFor(x=>x.Version).GreaterThan(0U);
    }
}
