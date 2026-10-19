using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Branches.Commands.Common;
using KLMN.Application.Branches.Responses;
using MediatR;

namespace KLMN.Application.Branches.Commands.UpdateBranchDef;

/// <summary>Branş Update iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class UpdateBranchDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateBranchDefCommand, BranchDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<BranchDefMutationResponse> Handle(UpdateBranchDefCommand request, CancellationToken ct)
    {
        var item = await BranchDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await BranchDefRules.EnsureUniqueCodeAsync(db, request.Code, item.Id, ct);
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;
        await db.SaveChangesAsync(ct);
        return new BranchDefMutationResponse(item.Id, item.Version);
    }
}
