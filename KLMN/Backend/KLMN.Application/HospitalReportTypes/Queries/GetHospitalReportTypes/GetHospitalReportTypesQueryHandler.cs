using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.HospitalReportTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypes;

/// <summary>Diğer tanım listeleriyle aynı arama ve sunucu sayfalamasını kullanır.</summary>
public sealed class GetHospitalReportTypesQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetHospitalReportTypesQuery, PagedResult<HospitalReportTypeResponse>>
{
    /// <summary>Silinmiş sağlık tesisi rapor türüları hariç tutar; Code/Name aramasını SQL'e uygular.</summary>
    public async Task<PagedResult<HospitalReportTypeResponse>> Handle(GetHospitalReportTypesQuery request, CancellationToken ct)
    {
        var query = db.HospitalReportTypes.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Hospital.Name.ToUpper().Contains(search) ||
                x.Hospital.Code.ToUpper().Contains(search) ||
                x.ReportType.Name.ToUpper().Contains(search) ||
                x.ReportType.Code.ToUpper().Contains(search));
        }
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Hospital.Name).ThenBy(x => x.ReportType.Name)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(HospitalReportTypeResponse.Projection).ToListAsync(ct);
        return new PagedResult<HospitalReportTypeResponse>
        { Items = items, TotalCount = total, PageNumber = request.PageNumber, PageSize = request.PageSize };
    }
}
