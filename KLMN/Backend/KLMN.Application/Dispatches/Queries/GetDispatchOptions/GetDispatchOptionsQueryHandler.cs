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
        var s=request.Search?.Trim()??"";
        // PostgreSQL ILIKE: sunucudan aranan seçenekler harf büyüklüğünden etkilenmez.
        // Escape ile kullanıcının girdiği % ve _ literal kabul edilir.
        var escaped=s.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_");
        var pattern="%"+escaped+"%";
        var id=request.SelectedId;
        // Eski militaryunitandoffice birleşik tanımı: ayrı iki tablo tek aramada.
        // Kind, iki farklı FK'yi tek dropdown'dan güvenli seçebilmek için gereklidir.
        if(request.Type=="senders")
        {
            var units=await db.MilitaryUnitDefs.IgnoreQueryFilters().AsNoTracking()
                .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Name,pattern,@"\")||x.Id==id)
                .OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,"Birlik · "+x.Name,x.Code,"unit"))
                .ToListAsync(ct);
            var offices=await db.MilitaryOfficeDefs.IgnoreQueryFilters().AsNoTracking()
                .Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Name,pattern,@"\")||x.Id==id)
                .OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,"Askerlik Şubesi · "+x.Name,x.Code,"office"))
                .ToListAsync(ct);
            return units.Concat(offices)
                .OrderByDescending(x=>x.Id==id)
                .ThenBy(x=>x.Label).Take(100).ToList();
        }
        // Üst limit 100; UI filtrelemesi API aramasıyla yenilenir.
        return request.Type switch
        {
            "countries" => await db.CountryDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Name,pattern,@"\")||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "towns" => await db.TownDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Name,pattern,@"\")||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "cities" => await db.CityDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Name,pattern,@"\")||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "hospitals" => await db.HospitalDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Name,pattern,@"\")||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "admissionTypes" => await db.HcAdmissionTypeDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Name,pattern,@"\")||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "reasons" => await db.ReasonForExaminationDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Description,pattern,@"\")||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Description).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Description)).ToListAsync(ct),
            "forces" => await db.ForcesCommandDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Name,pattern,@"\")||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name,x.Code)).ToListAsync(ct),
            "ranks" => await db.RankDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Name,pattern,@"\")||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "classes" => await db.MilitaryClassDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Name,pattern,@"\")||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "units" => await db.MilitaryUnitDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Name,pattern,@"\")||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name,x.Code)).ToListAsync(ct),
            "schools" => await db.MilitarySchoolDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Name,pattern,@"\")||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "offices" => await db.MilitaryOfficeDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Name,pattern,@"\")||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name,x.Code)).ToListAsync(ct),
            "missions" => await db.FlyingMissionDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Name,pattern,@"\")||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            "planes" => await db.PlaneTypeDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>!x.IsDeleted&&(x.IsActive||x.Id==id))
                .Where(x=>EF.Functions.ILike(x.Name,pattern,@"\")||x.Id==id).OrderByDescending(x=>x.Id==id).ThenBy(x=>x.Name).Take(100)
                .Select(x=>new DispatchOption(x.Id,x.Name)).ToListAsync(ct),
            _ => Array.Empty<DispatchOption>()
        };
    }
}
