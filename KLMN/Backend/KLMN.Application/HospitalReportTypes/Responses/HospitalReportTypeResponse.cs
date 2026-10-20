using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;

namespace KLMN.Application.HospitalReportTypes.Responses;

/// <summary>Sağlık tesisi ve rapor türü ilişkisi liste/detay yanıtı.</summary>
public sealed class HospitalReportTypeResponse
{
    public Guid Id { get; init; }
    public Guid HospitalDefId { get; init; }
    public string HospitalName { get; init; } = string.Empty;
    public Guid ReportTypeDefId { get; init; }
    public string ReportTypeName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public Guid? UpdatedBy { get; init; }
    public uint Version { get; init; }

    /// <summary>EF Core'da sunucu tarafında çevrilebilen DTO projeksiyonu.</summary>
    public static Expression<Func<HospitalReportType, HospitalReportTypeResponse>> Projection => x =>
        new HospitalReportTypeResponse
        {
            Id = x.Id,
            HospitalDefId = x.HospitalDefId, HospitalName = x.Hospital.Name,
            ReportTypeDefId = x.ReportTypeDefId, ReportTypeName = x.ReportType.Name,
            IsActive = x.IsActive,
            CreatedDate = x.CreatedDate, CreatedBy = x.CreatedBy,
            UpdatedDate = x.UpdatedDate, UpdatedBy = x.UpdatedBy, Version = x.Version
        };
}
