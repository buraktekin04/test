using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitarySchools.Queries.GetMilitarySchoolOptions;

/// <summary>Okul formu için aktif şehir/kuvvetleri ve düzenlemedeki mevcut seçimleri getirir.</summary>
public sealed class GetMilitarySchoolOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitarySchoolOptionsQuery, MilitarySchoolOptionsResponse>
{
    /// <summary>İlişkili tanımlara yazma/görüntüleme izni vermeden sınırlı seçenek verisi döner.</summary>
    public async Task<MilitarySchoolOptionsResponse> Handle(GetMilitarySchoolOptionsQuery request, CancellationToken ct)
    {
        var cities = await db.CityDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (x.IsActive || x.Id == request.SelectedCityId))
            .OrderBy(x => x.Name)
            .Select(x => new MilitarySchoolOptionResponse(x.Id, x.Code, x.Name))
            .ToListAsync(ct);
        var commands = await db.ForcesCommandDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (x.IsActive || x.Id == request.SelectedForcesCommandDefId))
            .OrderBy(x => x.Name)
            .Select(x => new MilitarySchoolOptionResponse(x.Id, x.Code, x.Name))
            .ToListAsync(ct);
        return new MilitarySchoolOptionsResponse(cities, commands);
    }
}
