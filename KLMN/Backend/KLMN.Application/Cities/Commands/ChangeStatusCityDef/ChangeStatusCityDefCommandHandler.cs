using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Cities.Commands.Common;
using KLMN.Application.Cities.Responses;
using MediatR;

namespace KLMN.Application.Cities.Commands.ChangeStatusCityDef;

/// <summary>Şehir ancak varsa bağlı olduğu ülke aktifken etkinleştirilebilir.</summary>
public sealed class ChangeStatusCityDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusCityDefCommand, CityDefMutationResponse>
{
    public async Task<CityDefMutationResponse> Handle(ChangeStatusCityDefCommand request, CancellationToken ct)
    {
        var item = await CityDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (request.IsActive)
            await CityDefRules.EnsureCountryAsync(db, item.CountryId, ct);
        else
        {
            await CityDefRules.EnsureNoTownsAsync(db, item.Id, deactivate: true, ct);
            await CityDefRules.EnsureNoMilitarySchoolsAsync(db, item.Id, deactivate: true, ct);
            await CityDefRules.EnsureNoMilitaryOfficesAsync(db, item.Id, deactivate: true, ct);
            await CityDefRules.EnsureNoHospitalsAsync(db, item.Id, true, ct);
        }
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new CityDefMutationResponse(item.Id, item.Version);
    }
}
