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
            "countries" => await db.CountryDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "towns" => await db.TownDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "cities" => await db.CityDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "hospitals" => await db.HospitalDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "admissionTypes" => await db.HcAdmissionTypeDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "reasons" => await db.ReasonForExaminationDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>x.Description.ToUpper().Contains(s)||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Description).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Description)).ToListAsync(ct),
            "forces" => await db.ForcesCommandDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "ranks" => await db.RankDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "classes" => await db.MilitaryClassDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "units" => await db.MilitaryUnitDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "schools" => await db.MilitarySchoolDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "offices" => await db.MilitaryOfficeDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "missions" => await db.FlyingMissionDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "planes" => await db.PlaneTypeDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>x.Name.ToUpper().Contains(s)||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            _ => Array.Empty<DispatchOption>()
        };
    }
}
