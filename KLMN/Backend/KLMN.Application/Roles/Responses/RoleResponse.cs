using KLMN.Domain.Entities.Identity;

namespace KLMN.Application.Roles.Responses;

/// <summary>Rol listeleme, detay ve yetki ekranlarının ortak güvenli DTO'sudur.</summary>
public sealed class RoleResponse
{
    /// <summary>Rol kaydının benzersiz anahtarıdır.</summary>
    public Guid Id { get; init; }
    /// <summary>Kullanıcıya gösterilen rol adıdır.</summary>
    public string Name { get; init; } = string.Empty;
    /// <summary>Yetkilendirmede kullanılan benzersiz teknik koddur.</summary>
    public string Code { get; init; } = string.Empty;
    /// <summary>Rolün isteğe bağlı açıklamasıdır.</summary>
    public string? Description { get; init; }
    /// <summary>Sistem tarafından korunmuş rol olduğunu belirtir.</summary>
    public bool IsSystemRole { get; init; }
    /// <summary>Rolün kullanılabilirliğini belirtir.</summary>
    public bool IsActive { get; init; }
    /// <summary>Rolün oluşturulma zamanıdır (UTC).</summary>
    public DateTime CreatedDate { get; init; }
    /// <summary>PostgreSQL xmin concurrency versiyonudur.</summary>
    public uint Version { get; init; }
    /// <summary>Rolü kullanan etkin ve silinmemiş kullanıcı sayısıdır.</summary>
    public int UserCount { get; init; }
    /// <summary>Role atanmış geçerli permission kodlarıdır.</summary>
    public IReadOnlyCollection<string> PermissionCodes { get; init; } = [];

    /// <summary>Liste ve detay işlemlerinde tutarlı EF Core DTO projeksiyonudur.</summary>

}
