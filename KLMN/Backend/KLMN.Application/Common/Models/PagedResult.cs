namespace KLMN.Application.Common.Models;

/// <summary>
/// Sayfalı sorguların standart response modelidir.
/// </summary>
public sealed class PagedResult<T>
{
    /// <summary>
    /// Sayfalanmış sorgudan elde edilen kayıtların koleksiyonudur.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; init; } = [];

    /// <summary>
    /// Sayfalama öncesinde filtre koşuluna uyan toplam kayıt sayısıdır.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// İstenen bir tabanlı sayfa numarasıdır.
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Tek sayfada döndürülecek kayıt sayısıdır.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Toplam kayıt ve sayfa boyutundan hesaplanan sayfa sayısıdır.
    /// </summary>
    public int TotalPages =>
        PageSize <= 0
            ? 0
            : (int)Math.Ceiling(
                TotalCount / (double)PageSize);
}
