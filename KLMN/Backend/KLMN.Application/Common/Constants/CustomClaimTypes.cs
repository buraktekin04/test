namespace KLMN.Application.Common.Constants;

/// <summary>KLMN authentication ve authorization altyapısındaki özel JWT claim adlarını tanımlar.</summary>
public static class CustomClaimTypes
{
    /// <summary>
    /// Kullanıcının bağlı olduğu organizasyon biriminin kimliğidir.
    /// </summary>
    public const string OrganizationUnitId = "organization_unit_id";
    /// <summary>
    /// JWT'ye yazılan etkin iznin claim türü veya permission kodudur.
    /// </summary>
    public const string Permission = "permission";

    /// <summary>
    /// Parola değişikliği veya logout-all sonrasında eski access token'ları
    /// anında geçersiz kılmak için kullanılan security stamp claim'idir.
    /// </summary>
    public const string SecurityStamp = "security_stamp";
}
