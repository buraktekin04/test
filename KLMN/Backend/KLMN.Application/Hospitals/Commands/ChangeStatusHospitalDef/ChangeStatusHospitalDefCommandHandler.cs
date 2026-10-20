using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Hospitals.Commands.Common;
using KLMN.Application.Hospitals.Responses;
using MediatR;

namespace KLMN.Application.Hospitals.Commands.ChangeStatusHospitalDef;

/// <summary>Sağlık Tesisi ChangeStatus iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class ChangeStatusHospitalDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusHospitalDefCommand, HospitalDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<HospitalDefMutationResponse> Handle(ChangeStatusHospitalDefCommand request, CancellationToken ct)
    {
        var item = await HospitalDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (request.IsActive)
            await HospitalDefRules.EnsureRelationsAsync(db, item.CityId, item.RefereeHospitalId,
                item.Id, ct);
        else await HospitalDefRules.EnsureNoDependentsAsync(db, item.Id, true, ct);
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new HospitalDefMutationResponse(item.Id, item.Version);
    }
}
