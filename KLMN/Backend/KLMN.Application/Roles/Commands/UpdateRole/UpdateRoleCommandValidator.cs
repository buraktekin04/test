using FluentValidation;

namespace KLMN.Application.Roles.Commands.UpdateRole;

/// <summary>Rol düzenleme ve optimistic concurrency doğrulamasıdır.</summary>
public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    /// <summary>Güncelleme için gerekli rol kimliği, versiyon ve alan kontrolleridir.</summary>
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100)
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage("Rol kodu yalnızca harf, rakam ve alt çizgi içerebilir.");
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
