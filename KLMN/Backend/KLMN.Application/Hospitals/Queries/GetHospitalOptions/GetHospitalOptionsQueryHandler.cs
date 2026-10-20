using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Hospitals.Queries.GetHospitalOptions;

/// <summary>Aktif tanımlar + düzenlenmekte olan kaydın seçili pasif tanımları.</summary>
public sealed class GetHospitalOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetHospitalOptionsQuery, HospitalOptionsResponse>
{
    public async Task<HospitalOptionsResponse> Handle(GetHospitalOptionsQuery request, CancellationToken ct)
    {
        var cities = await db.CityDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (x.IsActive || x.Id == request.SelectedCityId))
            .OrderBy(x => x.Name).Select(x => new HospitalOption(x.Id, x.Code, x.Name)).ToListAsync(ct);
        var hospitals = await db.HospitalDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (x.IsActive || x.Id == request.SelectedRefereeHospitalId))
            .OrderBy(x => x.Name).Select(x => new HospitalOption(x.Id, x.Code, x.Name)).ToListAsync(ct);
        return new HospitalOptionsResponse(cities, hospitals);
    }
}
