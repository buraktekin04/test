namespace KLMN.Application.Common.Constants;

/// <summary>KLMN authentication ve authorization altyapısındaki özel JWT claim adlarını tanımlar.</summary>
public static class CustomClaimTypes
{
    /// <summary>
    /// Kullanıcının bağlı olduğu birliğin kimliğidir.
    /// </summary>
    public const string MilitaryUnitDefId = "military_unit_def_id";
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
