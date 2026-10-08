namespace KLMN.Persistence.Configurations.Common;

/// <summary>EF Core named global query filter isimlerini merkezi olarak tanımlar.</summary>
public static class QueryFilterNames
{
    /// <summary>
    /// Silinmiş kayıtları sorgudan çıkaran EF Core global filtre adıdır.
    /// </summary>
    public const string SoftDelete = "SoftDeleteFilter";
    /// <summary>
    /// İş süreçlerinde pasif hale getirilen kayıtları sorgudan çıkaran filtre adıdır.
    /// </summary>
    public const string Active = "ActiveFilter";
}
