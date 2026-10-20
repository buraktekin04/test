using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExaminations.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ReasonForExaminations.Commands.Common;
/// <summary>Muayene sebebi kod, referans ve xmin doğrulama kuralları.</summary>
internal static class ReasonForExaminationDefRules
{
    public static async Task EnsureUniqueAsync(IKLMNDbContext db,string code,Guid? exclude,CancellationToken ct)
    {
        var key=code.Trim().ToUpperInvariant();
        if(await db.ReasonForExaminationDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted &&
            x.Code.ToUpper()==key && (!exclude.HasValue||x.Id!=exclude.Value),ct))
            throw new ReasonForExaminationConflictException("Muayene sebebi kodu kullanımda.");
    }
    public static async Task<ReasonForExaminationDef> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
    {
        var item=await db.ReasonForExaminationDefs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
            ?? throw new ReasonForExaminationNotFoundException();
        if(item.Version!=version) throw new DbUpdateConcurrencyException("Kayıt başka kullanıcı tarafından değiştirildi.");
        return item;
    }
    /// <summary>Aktif ilişkiler varken muayene sebebini pasifleştirmeyi engeller.</summary>
    public static async Task EnsureNoActiveLinksAsync(IKLMNDbContext db,Guid id,CancellationToken ct)
    {
        if(await db.ReasonForExamReportTypes.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.IsActive&&x.ReasonForExaminationDefId==id,ct)
        ||await db.ReasonForExamUnusedHcAdmissionTypes.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.IsActive&&x.ReasonForExaminationDefId==id,ct)
        ||await db.ReasonForExamProcedureDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.IsActive&&x.ReasonForExaminationDefId==id,ct)
        ||await db.ReasonForExamBranchDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.IsActive&&x.ReasonForExaminationDefId==id,ct))
            throw new ReasonForExaminationConflictException("Muayene sebebine bağlı aktif eşleştirmeler var.");
    }
    public static async Task EnsureNoLinksAsync(IKLMNDbContext db,Guid id,CancellationToken ct)
    {
        if(await db.ReasonForExamReportTypes.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.ReasonForExaminationDefId==id,ct)
        ||await db.ReasonForExamUnusedHcAdmissionTypes.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.ReasonForExaminationDefId==id,ct)
        ||await db.ReasonForExamProcedureDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.ReasonForExaminationDefId==id,ct)
        ||await db.ReasonForExamBranchDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.ReasonForExaminationDefId==id,ct))
            throw new ReasonForExaminationConflictException("Muayene sebebinin silinmemiş eşleştirmeleri var.");
    }
}
