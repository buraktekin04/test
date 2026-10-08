namespace KLMN.Persistence.Configurations.Common;

/// <summary>PostgreSQL partial index SQL filtrelerini merkezi olarak tutar.</summary>
public static class PostgreSqlIndexFilters
{
    /// <summary>
    /// PostgreSQL partial unique index'lerinde yalnızca silinmemiş kayıtları seçen SQL koşuludur.
    /// </summary>
    public const string NotDeleted = "\"IsDeleted\" = FALSE";
}
