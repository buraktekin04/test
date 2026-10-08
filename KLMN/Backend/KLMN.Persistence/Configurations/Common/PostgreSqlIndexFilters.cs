namespace KLMN.Persistence.Configurations.Common;

/// <summary>PostgreSQL partial index SQL filtrelerini merkezi olarak tutar.</summary>
public static class PostgreSqlIndexFilters
{
    public const string NotDeleted = "\"IsDeleted\" = FALSE";
}
