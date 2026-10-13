using FluentValidation;

namespace KLMN.Application.Auditing.Queries.GetAuditLogs;

/// <summary>Audit sorgularına sunucu filtre ve maliyet sınırı uygular.</summary>
public sealed class GetAuditLogsQueryValidator : AbstractValidator<GetAuditLogsQuery>
{
    private static readonly string[] Types =
        ["User", "Role", "MilitaryUnitDef", "ForcesCommandDef", "Permission",
         "UserRole", "RolePermission", "UserPermission", "UserAgreementPdf"];

    private static readonly string[] Actions =
        ["Create", "Update", "Delete", "Activate", "Deactivate"];

    public GetAuditLogsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.EntityType)
            .Must(x => x is null || Types.Contains(x))
            .WithMessage("Geçersiz audit varlık türü.");
        RuleFor(x => x.Action)
            .Must(x => x is null || Actions.Contains(x))
            .WithMessage("Geçersiz audit işlem tipi.");
        RuleFor(x => x.From)
            .Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Başlangıç UTC olmalıdır.");
        RuleFor(x => x.To)
            .Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Bitiş UTC olmalıdır.");
        RuleFor(x => x)
            .Must(x => !x.From.HasValue || !x.To.HasValue || x.From <= x.To)
            .WithMessage("Bitiş başlangıçtan önce olamaz.");
        RuleFor(x => x)
            .Must(x => !x.EntityId.HasValue || !string.IsNullOrWhiteSpace(x.EntityType))
            .WithMessage("Kayıt kimliğiyle sorguda varlık türü zorunludur.");
    }
}
