using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ProcedureTypes.Exceptions;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ProcedureTypes.Commands.Common;
/// <summary>Eski enum kodunu koruyan tekillik, ilişki ve xmin kuralları.</summary>
internal static class ProcedureTypeRules
{
    /// <summary>Silinmiş kodun dahi yeniden farklı anlamla kullanılmasını engeller.</summary>
    public static async Task EnsureNewCodeAsync(IKLMNDbContext db,int code,CancellationToken ct)
    {
        if(await db.ProcedureTypeDefs.IgnoreQueryFilters().AnyAsync(x=>x.Code==code,ct))
            throw new ProcedureTypeConflictException("Bu tetkik türü kodu kullanımda.");
    }
    /// <summary>İşlemin mevcut kayıt sürümüne ait olduğunu doğrular.</summary>
    public static async Task<ProcedureTypeDef> LoadAsync(IKLMNDbContext db,Guid id,uint version,CancellationToken ct)
    {
        var item=await db.ProcedureTypeDefs.IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id&&!x.IsDeleted,ct)
            ??throw new ProcedureTypeNotFoundException();
        if(item.Version!=version)throw new DbUpdateConcurrencyException("Tetkik türü başka kullanıcı tarafından güncellendi.");
        return item;
    }
    /// <summary>Bir türe aktif tetkik bağlıysa pasifleştirmeyi reddeder.</summary>
    public static async Task EnsureNoActiveProceduresAsync(IKLMNDbContext db,int code,CancellationToken ct)
    {
        if(await db.ProcedureDefs.IgnoreQueryFilters().AnyAsync(x=>x.ProcedureType==code&&!x.IsDeleted&&x.IsActive,ct))
            throw new ProcedureTypeConflictException("Aktif tetkikleri bulunan tür pasifleştirilemez.");
    }
    /// <summary>Tarihçede bile referans varsa türün silinmesini engeller.</summary>
    public static async Task EnsureNoProceduresAsync(IKLMNDbContext db,int code,CancellationToken ct)
    {
        if(await db.ProcedureDefs.IgnoreQueryFilters().AnyAsync(x=>x.ProcedureType==code,ct))
            throw new ProcedureTypeConflictException("Bu türe bağlı tetkikler bulunduğundan silinemez.");
    }
}
