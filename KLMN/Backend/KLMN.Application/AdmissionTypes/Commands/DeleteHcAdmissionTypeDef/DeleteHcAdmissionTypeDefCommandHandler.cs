using KLMN.Application.AdmissionTypes.Exceptions;
using Microsoft.EntityFrameworkCore;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.AdmissionTypes.Commands.Common;
using MediatR;

namespace KLMN.Application.AdmissionTypes.Commands.DeleteHcAdmissionTypeDef;

/// <summary>İşlemi güncel xmin ve audit/soft-delete interceptor'ı üzerinden yürütür.</summary>
public sealed class DeleteHcAdmissionTypeDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteHcAdmissionTypeDefCommand>
{
    /// <summary>İlgili işlem için geçerli kayıt sürümünü kontrol eder.</summary>
    public async Task Handle(
        DeleteHcAdmissionTypeDefCommand request, CancellationToken ct)
    {
        var item = await HcAdmissionTypeDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (await db.ReasonForExamUnusedHcAdmissionTypes.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && x.HcAdmissionTypeDefId == item.Id, ct))
            throw new AdmissionTypeConflictException("Silinmemiş muayene sebebi eşleştirmesi bulunan tanım silinemez.");
        if(await db.ICD10Diagnostics.IgnoreQueryFilters().AnyAsync(x=>
            !x.IsDeleted&&x.HcAdmissionTypeDefId==item.Id,ct))
           throw new AdmissionTypeConflictException("ICD10 SYY istatistik kaydı bağlı olan başvuru grubu silinemez.");
        // Kurul kararlarında kullanılmayacak olarak işaretlenen gruplar korunur.
        if(await db.HcUnusedAdmissionTypes.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.HcAdmissionTypeDefId==item.Id,ct))
            throw new AdmissionTypeConflictException("Kurul kararına bağlı silinmemiş başvuru grubu eşleştirmesi bulunuyor.");
        db.HcAdmissionTypeDefs.Remove(item);
        await db.SaveChangesAsync(ct);
    }
}
