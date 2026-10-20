using KLMN.Application.Branches.Exceptions;
using Microsoft.EntityFrameworkCore;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Branches.Commands.Common;
using KLMN.Application.Branches.Responses;
using MediatR;

namespace KLMN.Application.Branches.Commands.ChangeStatusBranchDef;

/// <summary>Branş ChangeStatus iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class ChangeStatusBranchDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusBranchDefCommand, BranchDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<BranchDefMutationResponse> Handle(ChangeStatusBranchDefCommand request, CancellationToken ct)
    {
        var item = await BranchDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (!request.IsActive && await db.ReasonForExamBranchDefs.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && x.IsActive && x.BranchDefId == item.Id, ct))
            throw new BranchConflictException("Aktif muayene sebebi eşleştirmesi bulunan tanım pasifleştirilemez.");
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new BranchDefMutationResponse(item.Id, item.Version);
    }
}
