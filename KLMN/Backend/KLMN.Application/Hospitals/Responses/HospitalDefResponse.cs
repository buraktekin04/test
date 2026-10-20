using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;

namespace KLMN.Application.Hospitals.Responses;

/// <summary>Sağlık tesisi liste/detay yanıtı: şehir ve hakem sağlık tesisi ilişkileriyle birlikte.</summary>
public sealed class HospitalDefResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Guid? CityId { get; init; }
    public string? CityName { get; init; }
    public Guid? RefereeHospitalId { get; init; }
    public string? RefereeHospitalName { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public Guid? UpdatedBy { get; init; }
    public uint Version { get; init; }

    /// <summary>EF Core'da sunucu tarafında çevrilebilen DTO projeksiyonu.</summary>
    public static Expression<Func<HospitalDef, HospitalDefResponse>> Projection => x =>
        new HospitalDefResponse
        {
            Id = x.Id, Code = x.Code, Name = x.Name,
            CityId = x.CityId, CityName = x.City == null ? null : x.City.Name,
            RefereeHospitalId = x.RefereeHospitalId,
            RefereeHospitalName = x.RefereeHospital == null ? null : x.RefereeHospital.Name,
            ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, IsActive = x.IsActive,
            CreatedDate = x.CreatedDate, CreatedBy = x.CreatedBy,
            UpdatedDate = x.UpdatedDate, UpdatedBy = x.UpdatedBy, Version = x.Version
        };
}
