namespace KLMN.Application.Users.Responses;

/// <summary>
/// Kullanıcı tanımlama/düzenleme görev yeri seçiminde kullanılan
/// organizasyon birimi seçeneğini temsil eden güvenli yanıt modelidir.
/// </summary>
public sealed record OrganizationUnitOptionResponse
{
    /// <summary>Birimin kullanıcıya atanacak benzersiz GUID değeridir.</summary>
    public required Guid Id { get; init; }

    /// <summary>Organizasyon biriminin kurumsal teknik kodudur.</summary>
    public required string Code { get; init; }

    /// <summary>Kullanıcıya gösterilen organizasyon birimi adıdır.</summary>
    public required string Name { get; init; }

    /// <summary>Üst organizasyon birimi; kök birimler için null olabilir.</summary>
    public Guid? ParentOrganizationUnitId { get; init; }
}
