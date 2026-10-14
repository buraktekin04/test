using FluentValidation;
namespace KLMN.Application.Towns.Commands.DeleteTownDef;
/// <summary>İlçe tanımı işlem veya doğrulama sözleşmesi; ilgili CQRS katmanında kullanılır.</summary>
public sealed class DeleteTownDefCommandValidator : AbstractValidator<DeleteTownDefCommand>
{
    public DeleteTownDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
