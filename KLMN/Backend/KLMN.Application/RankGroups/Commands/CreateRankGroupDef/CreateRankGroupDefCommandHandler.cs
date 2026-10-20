using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.RankGroups.Commands.Common;
using KLMN.Application.RankGroups.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;

namespace KLMN.Application.RankGroups.Commands.CreateRankGroupDef;

/// <summary>Rütbe Grubu Create iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class CreateRankGroupDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateRankGroupDefCommand, RankGroupDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<RankGroupDefMutationResponse> Handle(CreateRankGroupDefCommand request, CancellationToken ct)
    {
        await RankGroupDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        var item = new RankGroupDef
        {
            Code = request.Code.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            ShortName = string.IsNullOrWhiteSpace(request.ShortName) ? null : request.ShortName.Trim(),
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo
        };
        await db.RankGroupDefs.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new RankGroupDefMutationResponse(item.Id, item.Version);
    }
}
