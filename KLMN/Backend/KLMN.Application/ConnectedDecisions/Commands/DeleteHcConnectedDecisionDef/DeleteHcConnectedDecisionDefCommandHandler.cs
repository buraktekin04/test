using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ConnectedDecisions.Commands.Common;
using MediatR;

namespace KLMN.Application.ConnectedDecisions.Commands.DeleteHcConnectedDecisionDef;

/// <summary>Bağlı kurul kararı Delete iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class DeleteHcConnectedDecisionDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteHcConnectedDecisionDefCommand>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task Handle(DeleteHcConnectedDecisionDefCommand request, CancellationToken ct)
    {
        var item = await HcConnectedDecisionDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        db.HcConnectedDecisionDefs.Remove(item);
        await db.SaveChangesAsync(ct);
    }
}
