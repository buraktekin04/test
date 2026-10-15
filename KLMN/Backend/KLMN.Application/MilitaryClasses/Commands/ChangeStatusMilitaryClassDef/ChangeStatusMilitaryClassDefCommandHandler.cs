using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClasses.Commands.Common;
using KLMN.Application.MilitaryClasses.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using KLMN.Application.MilitaryClasses.Exceptions;

namespace KLMN.Application.MilitaryClasses.Commands.ChangeStatusMilitaryClassDef;

/// <summary>Kayıt yönetimi için BaseEntity.IsActive değerini değiştirir.</summary>
public sealed class ChangeStatusMilitaryClassDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusMilitaryClassDefCommand, MilitaryClassDefMutationResponse>
{
    /// <summary>Güncel xmin'i doğrular ve entity değişikliğini kaydeder.</summary>
    public async Task<MilitaryClassDefMutationResponse> Handle(ChangeStatusMilitaryClassDefCommand request, CancellationToken ct)
    {
        var item = await MilitaryClassDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (!request.IsActive && await db.MilitaryClassGroupRelations.IgnoreQueryFilters()
            .AnyAsync(x => x.MilitaryClassId == item.Id && x.IsActive && !x.IsDeleted, ct))
            throw new MilitaryClassConflictException("Aktif eşleştirmesi bulunan sınıf pasifleştirilemez.");
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new MilitaryClassDefMutationResponse(item.Id, item.Version);
    }
}
