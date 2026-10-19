using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ConnectedDecisions.Commands.Common;
using KLMN.Application.ConnectedDecisions.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;

namespace KLMN.Application.ConnectedDecisions.Commands.CreateHcConnectedDecisionDef;

/// <summary>Bağlı kurul kararı Create iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class CreateHcConnectedDecisionDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateHcConnectedDecisionDefCommand, HcConnectedDecisionDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<HcConnectedDecisionDefMutationResponse> Handle(CreateHcConnectedDecisionDefCommand request, CancellationToken ct)
    {
        await HcConnectedDecisionDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        var item = new HcConnectedDecisionDef
        {
            Code = request.Code.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo
        };
        await db.HcConnectedDecisionDefs.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new HcConnectedDecisionDefMutationResponse(item.Id, item.Version);
    }
}
