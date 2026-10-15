using FluentValidation;

namespace KLMN.Application.MilitaryClasses.Commands.DeleteMilitaryClassDef;

/// <summary>Güncel sürüm ve kayıt kimliği olmadan değişiklik yapılmasını engeller.</summary>
public sealed class DeleteMilitaryClassDefCommandValidator : AbstractValidator<DeleteMilitaryClassDefCommand>
{
    /// <summary>Id ve Version alanlarını doğrular.</summary>
    public DeleteMilitaryClassDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
