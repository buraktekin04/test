using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Branches.Commands.Common;
using MediatR;

namespace KLMN.Application.Branches.Commands.DeleteBranchDef;

/// <summary>Branş Delete iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class DeleteBranchDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteBranchDefCommand>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task Handle(DeleteBranchDefCommand request, CancellationToken ct)
    {
        var item = await BranchDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        db.BranchDefs.Remove(item);
        await db.SaveChangesAsync(ct);
    }
}
