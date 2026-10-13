using FluentValidation;

namespace KLMN.Application.Roles.Commands.DeleteRole;

/// <summary>Silme komutunun kimlik ve concurrency kontrolleridir.</summary>
public sealed class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    /// <summary>Soft-delete için gerekli alanları doğrular.</summary>
    public DeleteRoleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
