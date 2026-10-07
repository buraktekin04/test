namespace KLMN.Application.Common.Constants;

/// <summary>
/// EF Core named global query filter adlarını merkezi olarak tanımlar.
/// </summary>
public static class QueryFilterNames
{
    /// <summary>
    /// Soft delete kayıtlarını normal sorgulardan çıkaran filter adıdır.
    /// </summary>
    public const string SoftDeleteFilter = "SoftDeleteFilter";

    /// <summary>
    /// Pasif kayıtları normal sorgulardan çıkaran filter adıdır.
    /// </summary>
    public const string ActiveFilter = "ActiveFilter";
}
