using FluentValidation;

namespace KLMN.Application.Roles.Commands.UpdateRolePermissions;

/// <summary>Rol permission kaydetme komutunu doğrular.</summary>
public sealed class UpdateRolePermissionsCommandValidator : AbstractValidator<UpdateRolePermissionsCommand>
{
    /// <summary>Boş yetki listesinin geçerli olduğunu, tekrarlı kimliklerin olmadığını doğrular.</summary>
    public UpdateRolePermissionsCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
        RuleFor(x => x.PermissionIds).NotNull()
            .Must(x => x is not null && x.Count <= 500)
            .WithMessage("En fazla 500 yetki aynı anda yönetilebilir.");
        RuleFor(x => x.PermissionIds)
            .Must(x => x is not null && x.All(id => id != Guid.Empty)
                && x.Distinct().Count() == x.Count)
            .WithMessage("Yetki kimlikleri geçerli ve benzersiz olmalıdır.");
    }
}
