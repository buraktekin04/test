using FluentValidation;
using KLMN.Application.Roles.Commands;
using KLMN.Application.Roles.Queries;

namespace KLMN.Application.Roles.Validation;

/// <summary>Listeleme boyutu ve arama terimlerini doğrular.</summary>
public sealed class GetRolesQueryValidator : AbstractValidator<GetRolesQuery>
{
    /// <summary>Sunucu filtrelerine uygulanan güvenli sınırları tanımlar.</summary>
    public GetRolesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Search).MaximumLength(200);
    }
}

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

/// <summary>Rol düzenleme ve optimistic concurrency doğrulamasıdır.</summary>
public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    /// <summary>Güncelleme için gerekli rol kimliği, versiyon ve alan kontrolleridir.</summary>
    public UpdateRoleCommandValidator()
    {
        Include(new CreateRoleCommandValidatorAdapter());
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }

    /// <summary>Update alanlarını Create ile aynı uzunluk/kod kuralına bağlar.</summary>
    private sealed class CreateRoleCommandValidatorAdapter : AbstractValidator<UpdateRoleCommand>
    {
        /// <summary>Rol alanlarının doğrulama kurallarını tanımlar.</summary>
        public CreateRoleCommandValidatorAdapter()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Code).NotEmpty().MaximumLength(100)
                .Matches("^[a-zA-Z0-9_]+$")
                .WithMessage("Rol kodu yalnızca harf, rakam ve alt çizgi içerebilir.");
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }
}

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
