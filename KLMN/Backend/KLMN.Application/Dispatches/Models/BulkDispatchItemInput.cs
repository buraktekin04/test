namespace KLMN.Application.Dispatches.Models;

/// <summary>Dosyadaki tek sevkin mevcut CreateDispatch ile ortak veri sözleşmesi.</summary>
public sealed class BulkDispatchItemInput
{
    public DispatchPersonInput Person { get; init; } = null!;
    public DispatchDetailsInput Dispatch { get; init; } = null!;

    /// <summary>
    /// Eski MSKBS'deki SenderChair / muayeneyeGonderenMakam GUID değeri.
    /// Birlik/şube türü yeni GUID üretilmeden mevcut tablolardan doğrulanır.
    /// </summary>
    public Guid? LegacySenderChairId { get; init; }
}
