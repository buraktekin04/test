using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Hospitals.Commands.Common;
using KLMN.Application.Hospitals.Responses;
using MediatR;

namespace KLMN.Application.Hospitals.Commands.UpdateHospitalDef;

/// <summary>Sağlık Tesisi Update iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class UpdateHospitalDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateHospitalDefCommand, HospitalDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<HospitalDefMutationResponse> Handle(UpdateHospitalDefCommand request, CancellationToken ct)
    {
        var item = await HospitalDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await HospitalDefRules.EnsureUniqueCodeAsync(db, request.Code, item.Id, ct);
        await HospitalDefRules.EnsureRelationsAsync(db, request.CityId, request.RefereeHospitalId,
            item.Id, ct, item.CityId, item.RefereeHospitalId);
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.CityId = request.CityId;
        item.RefereeHospitalId = request.RefereeHospitalId;
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;
        await db.SaveChangesAsync(ct);
        return new HospitalDefMutationResponse(item.Id, item.Version);
    }
}
