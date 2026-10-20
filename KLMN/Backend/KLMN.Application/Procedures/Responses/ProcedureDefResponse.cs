using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.Procedures.Responses;
/// <summary>Tetkik Tanımları liste ve detay yanıtı; eski ProcedureDef alan adları korunur.</summary>
public sealed class ProcedureDefResponse
{
    public Guid Id { get; init; }
    public string ProcedureCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool RequiresNumericResult { get; init; }
    public bool RequiresReferenceRange { get; init; }
    public bool RequiresDeviationInfo { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int ProcedureType { get; init; }
    public string ProcedureTypeName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public Guid? UpdatedBy { get; init; }
    public uint Version { get; init; }

    /// <summary>Liste ve detay sorgularının EF Core üzerinde projekte ettiği alanlar.</summary>
    public static Expression<Func<ProcedureDef,ProcedureDefResponse>> Projection => x=> new ProcedureDefResponse
    {
        Id=x.Id,ProcedureCode=x.ProcedureCode,Name=x.Name,RequiresNumericResult=x.RequiresNumericResult,
        RequiresReferenceRange=x.RequiresReferenceRange,RequiresDeviationInfo=x.RequiresDeviationInfo,
        StartDate=x.StartDate,EndDate=x.EndDate,ProcedureType=x.ProcedureType,
        ProcedureTypeName=x.TypeDefinition.Name,IsActive=x.IsActive,CreatedDate=x.CreatedDate,
        CreatedBy=x.CreatedBy,UpdatedDate=x.UpdatedDate,UpdatedBy=x.UpdatedBy,Version=x.Version
    };
}
