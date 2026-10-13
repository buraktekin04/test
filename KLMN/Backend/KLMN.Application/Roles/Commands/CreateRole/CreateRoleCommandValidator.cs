using FluentValidation;

namespace KLMN.Application.Roles.Commands.CreateRole;

/// <summary>Yeni rol oluşturma input kurallarıdır.</summary>
public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    /// <summary>Ad, kod ve açıklama kontrolleridir.</summary>
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100)
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage("Rol kodu yalnızca harf, rakam ve alt çizgi içerebilir.");
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
