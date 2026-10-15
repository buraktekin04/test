using FluentValidation;

namespace KLMN.Application.MilitaryClasses.Commands.ChangeStatusMilitaryClassDef;

/// <summary>Güncel sürüm ve kayıt kimliği olmadan değişiklik yapılmasını engeller.</summary>
public sealed class ChangeStatusMilitaryClassDefCommandValidator : AbstractValidator<ChangeStatusMilitaryClassDefCommand>
{
    /// <summary>Id ve Version alanlarını doğrular.</summary>
    public ChangeStatusMilitaryClassDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
