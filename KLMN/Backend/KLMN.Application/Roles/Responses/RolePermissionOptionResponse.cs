

namespace KLMN.Application.Roles.Responses;

/// <summary>Rolün izin panelindeki modül bazlı gösterim seçeneğidir.</summary>
public sealed record RolePermissionOptionResponse(
    Guid Id,
    string Code,
    string Name,
    string Module,
    string? Description,
    int SortOrder);
