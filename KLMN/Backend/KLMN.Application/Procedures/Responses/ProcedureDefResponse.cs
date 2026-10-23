using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.Procedures.Responses;
/// <summary>Tetkik Tanımları liste ve detay yanıtı; iş alanlarını ve standart ValidFrom/ValidTo tarihlerini döndürür.</summary>
public sealed class ProcedureDefResponse
{
    public Guid Id { get; init; }
    public string ProcedureCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool RequiresNumericResult { get; init; }
    public bool RequiresReferenceRange { get; init; }
    public bool RequiresDeviationInfo { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public int ProcedureType { get; init; }
    public string ProcedureTypeName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public Guid? UpdatedBy { get; init; }
    public uint Version { get; init; }

    /// <summary>Liste ve detay sorgularının EF Core üzerinde projekte ettiği alanlar.</summary>

}
