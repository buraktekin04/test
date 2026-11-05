namespace KLMN.Application.Dispatches.Responses;

/// <summary>Toplu sevk sonucunda satır bazlı durum; kimlik numarası yalnız maskeli gösterilir.</summary>
public sealed class BulkDispatchRowResponse
{
    public int RowNumber { get; init; }
    public string IdentityMasked { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Surname { get; init; } = string.Empty;
    public Guid? DispatchId { get; init; }
    public long? DispatchNo { get; init; }
    public bool Success { get; init; }
    public int ResultCode { get; init; }
    public string Message { get; init; } = string.Empty;
}
