using FluentValidation;

namespace KLMN.Application.Roles.Commands.ChangeRoleStatus;

/// <summary>Aktiflik değişikliğinin kimlik ve versiyon kontrolleridir.</summary>
public sealed class ChangeRoleStatusCommandValidator : AbstractValidator<ChangeRoleStatusCommand>
{
    /// <summary>Aktiflik doğrulama kurallarını tanımlar.</summary>
    public ChangeRoleStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
