using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Ranks.Commands.Common;
using KLMN.Application.Ranks.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;

namespace KLMN.Application.Ranks.Commands.CreateRankDef;

/// <summary>Rütbe Create iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class CreateRankDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateRankDefCommand, RankDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<RankDefMutationResponse> Handle(CreateRankDefCommand request, CancellationToken ct)
    {
        await RankDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        await RankDefRules.EnsureGroupAsync(db, request.RankGroupId, ct);
        var item = new RankDef
        {
            Code = request.Code.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            ShortName = string.IsNullOrWhiteSpace(request.ShortName) ? null : request.ShortName.Trim(),
            Type = request.Type,
            RankGroupId = request.RankGroupId,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo
        };
        await db.RankDefs.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new RankDefMutationResponse(item.Id, item.Version);
    }
}
