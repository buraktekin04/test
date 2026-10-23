using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.Hospitals.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Hospitals.Queries.GetHospitalDefs;

/// <summary>Diğer tanım listeleriyle aynı arama ve sunucu sayfalamasını kullanır.</summary>
public sealed class GetHospitalDefsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetHospitalDefsQuery, PagedResult<HospitalDefResponse>>
{
    /// <summary>Silinmiş sağlık tesisiları hariç tutar; Code/Name aramasını SQL'e uygular.</summary>
    public async Task<PagedResult<HospitalDefResponse>> Handle(GetHospitalDefsQuery request, CancellationToken ct)
    {
        var query = db.HospitalDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.ToUpper().Contains(search) ||
                x.Name.ToUpper().Contains(search) ||
                (x.City != null && x.City.Name.ToUpper().Contains(search)));
        }
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(x =>
        new HospitalDefResponse
        {
            Id = x.Id, Code = x.Code, Name = x.Name,
            CityId = x.CityId, CityName = x.City == null ? null : x.City.Name,
            RefereeHospitalId = x.RefereeHospitalId,
            RefereeHospitalName = x.RefereeHospital == null ? null : x.RefereeHospital.Name,
            ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, IsActive = x.IsActive,
            CreatedDate = x.CreatedDate, CreatedBy = x.CreatedBy,
            UpdatedDate = x.UpdatedDate, UpdatedBy = x.UpdatedBy, Version = x.Version
        }).ToListAsync(ct);
        return new PagedResult<HospitalDefResponse>
        { Items = items, TotalCount = total, PageNumber = request.PageNumber, PageSize = request.PageSize };
    }
}
