namespace KLMN.Application.Users.Responses;

/// <summary>
/// Kullanıcı tanımlama/düzenleme görev yeri seçiminde kullanılan
/// birlik seçeneğini temsil eden güvenli yanıt modelidir.
/// </summary>
public sealed record MilitaryUnitDefOptionResponse
{
    /// <summary>Birimin kullanıcıya atanacak benzersiz GUID değeridir.</summary>
    public required Guid Id { get; init; }

    /// <summary>Birliğin kurumsal teknik kodudur.</summary>
    public required string Code { get; init; }

    /// <summary>Kullanıcıya gösterilen birlik adıdır.</summary>
    public required string Name { get; init; }

    /// <summary>Üst birlik; kök birimler için null olabilir.</summary>
    public Guid? ConnectedTopUnity { get; init; }
}
