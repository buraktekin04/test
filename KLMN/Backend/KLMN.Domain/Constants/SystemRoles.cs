namespace KLMN.Domain.Constants;

/// <summary>
/// KLMN sisteminde özel anlam taşıyan rol kodlarını tanımlar.
/// </summary>
public static class SystemRoles
{
    /// <summary>
    /// Tam yetkili sistem yöneticisi rol kodudur.
    /// </summary>
    public const string Admin = "ADMIN";

    /// <summary>
    /// Standart kullanıcı sistem rol kodudur.
    /// </summary>
    public const string StandardUser = "STANDARD_USER";

    /// <summary>
    /// İnceleme görevlisi rol kodudur.
    /// </summary>
    public const string InvestigationOfficer = "INVESTIGATION_OFFICER";

    /// <summary>
    /// Şube müdürü rol kodudur.
    /// </summary>
    public const string BranchManager = "BRANCH_MANAGER";
}
