using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcDecisions.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.HcDecisions.Commands.Common;
/// <summary>Kurul kararı benzersizliği, referans koruması ve xmin kuralları.</summary>
internal static class HcDecisionDefRules
{
    public static async Task<HcDecisionDef> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
    {
        var item=await db.HcDecisionDefs.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
          ?? throw new HcDecisionDefNotFoundException();
        if(item.Version!=version)throw new DbUpdateConcurrencyException("Kurul kararı başka kullanıcı tarafından değiştirildi.");
        return item;
    }
    public static async Task EnsureUniqueAsync(IKLMNDbContext db,int code,Guid? exclude,CancellationToken ct)
    {
        if(await db.HcDecisionDefs.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.Code==code&&(!exclude.HasValue||x.Id!=exclude.Value),ct))
            throw new HcDecisionDefConflictException("Karar kodu zaten kayıtlı.");
    }
    /// <summary>Silmede tüm, pasifleştirmede yalnız etkin alt ilişkileri denetler; ilişkiler tek tek kapatılır.</summary>
    public static async Task EnsureNoLinksAsync(IKLMNDbContext db,Guid id,bool onlyActive,CancellationToken ct)
    {
        if(await db.HcUnusedReasonForExams.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.HcDecisionDefId==id&&(!onlyActive||x.IsActive),ct)
        ||await db.HcUnusedAdmissionTypes.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.HcDecisionDefId==id&&(!onlyActive||x.IsActive),ct)
        ||await db.HcUnusedDecisions.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&(x.HcDecisionDefId==id||x.UnusedHcDecisionDefId==id)&&(!onlyActive||x.IsActive),ct)
        ||await db.HcConnectedDecisions.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.HcDecisionDefId==id&&(!onlyActive||x.IsActive),ct))
            throw new HcDecisionDefConflictException(onlyActive?"Karara bağlı aktif eşleştirmeler bulunuyor.":"Karara bağlı silinmemiş eşleştirmeler bulunuyor.");
    }
}
