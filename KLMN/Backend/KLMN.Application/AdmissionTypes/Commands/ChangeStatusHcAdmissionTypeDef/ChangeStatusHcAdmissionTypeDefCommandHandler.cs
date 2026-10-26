using KLMN.Application.AdmissionTypes.Exceptions;
using Microsoft.EntityFrameworkCore;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.AdmissionTypes.Commands.Common;
using KLMN.Application.AdmissionTypes.Responses;
using MediatR;

namespace KLMN.Application.AdmissionTypes.Commands.ChangeStatusHcAdmissionTypeDef;

/// <summary>İşlemi güncel xmin ve audit/soft-delete interceptor'ı üzerinden yürütür.</summary>
public sealed class ChangeStatusHcAdmissionTypeDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusHcAdmissionTypeDefCommand, HcAdmissionTypeDefMutationResponse>
{
    /// <summary>İlgili işlem için geçerli kayıt sürümünü kontrol eder.</summary>
    public async Task<HcAdmissionTypeDefMutationResponse> Handle(
        ChangeStatusHcAdmissionTypeDefCommand request, CancellationToken ct)
    {
        var item = await HcAdmissionTypeDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (!request.IsActive && await db.ReasonForExamUnusedHcAdmissionTypes.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && x.IsActive && x.HcAdmissionTypeDefId == item.Id, ct))
            throw new AdmissionTypeConflictException("Aktif muayene sebebi eşleştirmesi bulunan tanım pasifleştirilemez.");
        if(!request.IsActive && await db.ICD10Diagnostics.IgnoreQueryFilters().AnyAsync(x=>
           !x.IsDeleted&&x.IsActive&&x.HcAdmissionTypeDefId==item.Id,ct))
            throw new AdmissionTypeConflictException("Aktif ICD10 istatistik eşleştirmesi bulunan başvuru grubu pasifleştirilemez.");
        if(!request.IsActive && await db.HcUnusedAdmissionTypes.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.IsActive&&x.HcAdmissionTypeDefId==item.Id,ct))
            throw new AdmissionTypeConflictException("Kurul kararına bağlı aktif başvuru grubu eşleştirmesi bulunuyor.");
        if(!request.IsActive && await db.SYYDiagnosisDecisions.IgnoreQueryFilters().AnyAsync(x=>!x.IsDeleted&&x.IsActive&&x.HcAdmissionTypeDefId==item.Id,ct))
           throw new AdmissionTypeConflictException("Başvuru grubuna bağlı aktif SYY karar istatistiği bulunuyor.");
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new HcAdmissionTypeDefMutationResponse(item.Id, item.Version);
    }
}
