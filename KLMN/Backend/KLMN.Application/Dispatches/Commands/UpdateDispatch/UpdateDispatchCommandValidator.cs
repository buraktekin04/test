using FluentValidation;
namespace KLMN.Application.Dispatches.Commands.UpdateDispatch;
/// <summary>Sevk güncelleme komutunun kimlik ve veri denetimi.</summary>
public sealed class UpdateDispatchCommandValidator : AbstractValidator<UpdateDispatchCommand>
{
    public UpdateDispatchCommandValidator()
    {
        RuleFor(x=>x.Id).NotEmpty();
        RuleFor(x=>x.Version).GreaterThan(0U);
        RuleFor(x=>x.Person).NotNull();
        RuleFor(x=>x.Dispatch).NotNull();
    }
}
