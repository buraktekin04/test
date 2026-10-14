using FluentValidation;
namespace KLMN.Application.Towns.Commands.ChangeStatusTownDef;
/// <summary>İlçe tanımı işlem veya doğrulama sözleşmesi; ilgili CQRS katmanında kullanılır.</summary>
public sealed class ChangeStatusTownDefCommandValidator : AbstractValidator<ChangeStatusTownDefCommand>
{
    public ChangeStatusTownDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
