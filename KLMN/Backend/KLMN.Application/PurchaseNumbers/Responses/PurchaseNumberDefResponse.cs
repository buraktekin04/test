using KLMN.Domain.Entities.PurchaseNumbers;

namespace KLMN.Application.PurchaseNumbers.Responses;

public sealed class PurchaseNumberDefResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string SupplyDescriptionName { get; init; } = string.Empty;
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public DateTime? HealthReportEndDate { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public Guid? UpdatedBy { get; init; }
    public uint Version { get; init; }


}
