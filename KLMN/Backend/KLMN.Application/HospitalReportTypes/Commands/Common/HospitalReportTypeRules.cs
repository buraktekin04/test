using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HospitalReportTypes.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.HospitalReportTypes.Commands.Common;

/// <summary>Sağlık tesisi–rapor türü çifti tekilliği ve xmin concurrency iş kuralları.</summary>
internal static class HospitalReportTypeRules
{
    /// <summary>Aynı sağlık tesisi ve rapor türü iki kez bağlanamaz.</summary>
    public static async Task EnsureUniquePairAsync(IKLMNDbContext db, Guid hospitalId,
        Guid reportTypeId, Guid? excludedId, CancellationToken ct)
    {
        if (await db.HospitalReportTypes.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && x.HospitalDefId == hospitalId && x.ReportTypeDefId == reportTypeId &&
            (!excludedId.HasValue || x.Id != excludedId.Value), ct))
            throw new HospitalReportTypeConflictException("Bu sağlık tesisi ve rapor türü zaten eşleştirilmiş.");
    }

    /// <summary>Yeni seçilen ilişkiler aktif olmalı; eski pasif bağlantı düzenlemede korunur.</summary>
    public static async Task EnsureReferencesAsync(IKLMNDbContext db, Guid hospitalId, Guid reportId,
        CancellationToken ct, Guid? previousHospitalId = null, Guid? previousReportId = null)
    {
        if (hospitalId != previousHospitalId &&
            !await db.HospitalDefs.AnyAsync(x => x.Id == hospitalId && x.IsActive && !x.IsDeleted, ct))
            throw new HospitalReportTypeConflictException("Aktif bir sağlık tesisi seçiniz.");
        if (reportId != previousReportId &&
            !await db.ReportTypeDefs.AnyAsync(x => x.Id == reportId && x.IsActive && !x.IsDeleted, ct))
            throw new HospitalReportTypeConflictException("Aktif bir rapor türü seçiniz.");
    }

    /// <summary>Güncel xmin'i doğrular; eski kayıt sürümüyle değişikliği engeller.</summary>
    public static async Task<HospitalReportType> LoadForChangeAsync(
        IKLMNDbContext db, Guid id, uint version, CancellationToken ct)
    {
        var item = await db.HospitalReportTypes.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new HospitalReportTypeNotFoundException();
        if (item.Version != version)
            throw new DbUpdateConcurrencyException("Eşleştirme kaydı başka kullanıcı tarafından değiştirildi; listeyi yenileyiniz.");
        return item;
    }
}
