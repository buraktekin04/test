using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Towns.Commands.Common;
using KLMN.Application.Towns.Responses;
using MediatR;

namespace KLMN.Application.Towns.Commands.ChangeStatusTownDef;

/// <summary>İlçe ancak varsa bağlı olduğu şehir aktifken etkinleştirilebilir.</summary>
public sealed class ChangeStatusTownDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusTownDefCommand, TownDefMutationResponse>
{
    public async Task<TownDefMutationResponse> Handle(ChangeStatusTownDefCommand request, CancellationToken ct)
    {
        var item = await TownDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (request.IsActive)
            await TownDefRules.EnsureCityAsync(db, item.CityId, ct);
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new TownDefMutationResponse(item.Id, item.Version);
    }
}
