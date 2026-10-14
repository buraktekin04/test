using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Countries.Commands.Common;
using KLMN.Application.Countries.Responses;
using MediatR;

namespace KLMN.Application.Countries.Commands.ChangeStatusCountryDef;

/// <summary>Aktif şehre sahip ülke pasifleştirilemez; yeniden aktivasyon serbesttir.</summary>
public sealed class ChangeStatusCountryDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusCountryDefCommand, CountryDefMutationResponse>
{
    public async Task<CountryDefMutationResponse> Handle(ChangeStatusCountryDefCommand request, CancellationToken ct)
    {
        var item = await CountryDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (!request.IsActive)
            await CountryDefRules.EnsureNoCitiesAsync(db, item.Id, deactivate: true, ct);
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new CountryDefMutationResponse(item.Id, item.Version);
    }
}
