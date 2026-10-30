using FluentValidation;
namespace KLMN.Application.Dispatches.Commands.CreateDispatch;
/// <summary>Sevk girişinde temel istek alanları.</summary>
public sealed class CreateDispatchCommandValidator : AbstractValidator<CreateDispatchCommand>
{
    public CreateDispatchCommandValidator()
    {
        RuleFor(x=>x.Person).NotNull();
        RuleFor(x=>x.Dispatch).NotNull();
    }
}
