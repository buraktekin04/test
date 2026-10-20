using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.HospitalReportTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypeGroups;

/// <summary>
/// Önce tesis kimliklerini SQL üzerinde sayfalar; seçilen her tesisin eşleştirmelerini
/// daha sonra getirir. Böylece tesisin raporları farklı sayfalara bölünmez.
/// </summary>
public sealed class GetHospitalReportTypeGroupsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetHospitalReportTypeGroupsQuery, PagedResult<HospitalReportTypeGroupResponse>>
{
    public async Task<PagedResult<HospitalReportTypeGroupResponse>> Handle(
        GetHospitalReportTypeGroupsQuery request, CancellationToken ct)
    {
        var links = db.HospitalReportTypes.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (request.IncludeInactive || request.IsActive == false || x.IsActive));

        if (request.IsActive.HasValue)
            links = links.Where(x => x.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            links = links.Where(x =>
                x.Hospital.Name.ToUpper().Contains(search) ||
                x.Hospital.Code.ToUpper().Contains(search) ||
                x.ReportType.Name.ToUpper().Contains(search) ||
                x.ReportType.Code.ToUpper().Contains(search));
        }

        // Sayfalama tekil tesisler üzerindedir, eşleştirmeler üzerinde değil.
        var hospitals = links.Select(x => new
            { x.HospitalDefId, Name = x.Hospital.Name, Code = x.Hospital.Code })
            .Distinct();
        var count = await hospitals.CountAsync(ct);
        var ids = await hospitals.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .ThenBy(x => x.HospitalDefId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => x.HospitalDefId).ToListAsync(ct);

        if (ids.Count == 0)
            return new PagedResult<HospitalReportTypeGroupResponse>
            {
                Items = [], TotalCount = count,
                PageNumber = request.PageNumber, PageSize = request.PageSize
            };

        var rows = await links.Where(x => ids.Contains(x.HospitalDefId))
            .OrderBy(x => x.ReportType.Name).ThenBy(x => x.ReportType.Code)
            .Select(HospitalReportTypeResponse.Projection).ToListAsync(ct);
        var byHospital = rows.GroupBy(x => x.HospitalDefId).ToDictionary(x => x.Key,
            x => (IReadOnlyList<HospitalReportTypeResponse>)x.ToList());

        var result = ids.Select(id =>
        {
            var reports = byHospital[id];
            var first = reports[0];
            return new HospitalReportTypeGroupResponse(
                id, first.HospitalCode, first.HospitalName, reports);
        }).ToList();

        return new PagedResult<HospitalReportTypeGroupResponse>
        {
            Items = result, TotalCount = count,
            PageNumber = request.PageNumber, PageSize = request.PageSize
        };
    }
}
