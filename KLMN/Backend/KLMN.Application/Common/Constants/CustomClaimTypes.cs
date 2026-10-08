namespace KLMN.Application.Common.Constants;

/// <summary>KLMN authentication ve authorization altyapısındaki özel JWT claim adlarını tanımlar.</summary>
public static class CustomClaimTypes
{
    public const string OrganizationUnitId = "organization_unit_id";
    public const string Permission = "permission";

    /// <summary>
    /// Parola değişikliği veya logout-all sonrasında eski access token'ları
    /// anında geçersiz kılmak için kullanılan security stamp claim'idir.
    /// </summary>
    public const string SecurityStamp = "security_stamp";
}
