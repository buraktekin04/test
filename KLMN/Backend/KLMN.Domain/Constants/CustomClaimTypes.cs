namespace KLMN.Domain.Constants;

/// <summary>
/// JWT içerisinde kullanılan KLMN özel claim adlarını tanımlar.
/// </summary>
public static class CustomClaimTypes
{
    /// <summary>
    /// Kullanıcının organizasyon birimi kimliği claim adıdır.
    /// </summary>
    public const string OrganizationUnitId = "organization_unit_id";

    /// <summary>
    /// Permission claim adıdır.
    /// </summary>
    public const string Permission = "permission";

    /// <summary>
    /// Security stamp claim adıdır.
    /// </summary>
    public const string SecurityStamp = "security_stamp";
}
