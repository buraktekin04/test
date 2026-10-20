using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Hospitals.Commands.Common;
using KLMN.Application.Hospitals.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;

namespace KLMN.Application.Hospitals.Commands.CreateHospitalDef;

/// <summary>Sağlık Tesisi Create iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class CreateHospitalDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateHospitalDefCommand, HospitalDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<HospitalDefMutationResponse> Handle(CreateHospitalDefCommand request, CancellationToken ct)
    {
        await HospitalDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        await HospitalDefRules.EnsureRelationsAsync(db, request.CityId, request.RefereeHospitalId, null, ct);
        var item = new HospitalDef
        {
            Code = request.Code.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            CityId = request.CityId,
            RefereeHospitalId = request.RefereeHospitalId,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo
        };
        await db.HospitalDefs.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new HospitalDefMutationResponse(item.Id, item.Version);
    }
}
