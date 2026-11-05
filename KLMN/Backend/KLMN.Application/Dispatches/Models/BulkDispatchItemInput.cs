using KLMN.Application.Dispatches.Models;

namespace KLMN.Application.Dispatches.Models;

/// <summary>Dosyadaki tek sevkin mevcut CreateDispatch ile ortak veri sözleşmesi.</summary>
public sealed class BulkDispatchItemInput
{
    public DispatchPersonInput Person { get; init; } = null!;
    public DispatchDetailsInput Dispatch { get; init; } = null!;
}
