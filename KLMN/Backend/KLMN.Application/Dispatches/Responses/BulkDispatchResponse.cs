namespace KLMN.Application.Dispatches.Responses;

/// <summary>Her satır için bağımsız sonuç ve toplam başarı/hata sayısı.</summary>
public sealed class BulkDispatchResponse
{
    public int TotalCount { get; init; }
    public int SuccessCount { get; init; }
    public int FailureCount { get; init; }
    public IReadOnlyList<BulkDispatchRowResponse> Rows { get; init; } = [];
}
