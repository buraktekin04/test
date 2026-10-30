using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Dispatches.Queries.GetDispatchOptions;

/// <summary>Aktif ve silinmemiş referansları SQL'de arar; sınırsız tanım listesi çekmez.</summary>
public sealed class GetDispatchOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetDispatchOptionsQuery,IReadOnlyList<DispatchOption>>
{
    public async Task<IReadOnlyList<DispatchOption>> Handle(GetDispatchOptionsQuery request,CancellationToken ct)
    {
        var s=request.Search?.Trim().ToUpperInvariant()??"";
        var id=request.SelectedId;
        // Üst limit 100; UI filtrelemesi API aramasıyla yenilenir.
        return request.Type switch
        {
            "countries" => await db.CountryDefs.AsNoTracking().Where(x=>x.IsActive||x.Id==id)
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "towns" => await db.TownDefs.AsNoTracking().Where(x=>x.IsActive||x.Id==id)
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "cities" => await db.CityDefs.AsNoTracking().Where(x=>x.IsActive||x.Id==id)
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "hospitals" => await db.HospitalDefs.AsNoTracking().Where(x=>x.IsActive||x.Id==id)
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "admissionTypes" => await db.HcAdmissionTypeDefs.AsNoTracking().Where(x=>x.IsActive||x.Id==id)
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "reasons" => await db.ReasonForExaminationDefs.AsNoTracking().Where(x=>x.IsActive||x.Id==id)
                .Where(x=>x.Description.ToUpper().Contains(s)||x.Id==id).OrderBy(x=>x.Description).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Description)).ToListAsync(ct),
            "forces" => await db.ForcesCommandDefs.AsNoTracking().Where(x=>x.IsActive||x.Id==id)
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "ranks" => await db.RankDefs.AsNoTracking().Where(x=>x.IsActive||x.Id==id)
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "classes" => await db.MilitaryClassDefs.AsNoTracking().Where(x=>x.IsActive||x.Id==id)
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "units" => await db.MilitaryUnitDefs.AsNoTracking().Where(x=>x.IsActive||x.Id==id)
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "schools" => await db.MilitarySchoolDefs.AsNoTracking().Where(x=>x.IsActive||x.Id==id)
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "offices" => await db.MilitaryOfficeDefs.AsNoTracking().Where(x=>x.IsActive||x.Id==id)
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "missions" => await db.FlyingMissionDefs.AsNoTracking().Where(x=>x.IsActive||x.Id==id)
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "planes" => await db.PlaneTypeDefs.AsNoTracking().Where(x=>x.IsActive||x.Id==id)
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            _ => Array.Empty<DispatchOption>()
        };
    }
}
