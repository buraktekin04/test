using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypeOptions;

/// <summary>Yalnız aktif seçimler ve düzenlenen satırda önceden seçilmiş pasif değerler.</summary>
public sealed class GetHospitalReportTypeOptionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetHospitalReportTypeOptionsQuery, HospitalReportTypeOptionsResponse>
{
    public async Task<HospitalReportTypeOptionsResponse> Handle(
        GetHospitalReportTypeOptionsQuery request, CancellationToken ct)
    {
        var hospitals = await db.HospitalDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (x.IsActive || x.Id == request.SelectedHospitalDefId))
            .OrderBy(x => x.Name).Select(x => new DefinitionOption(x.Id, x.Code, x.Name)).ToListAsync(ct);
        var reports = await db.ReportTypeDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted && (x.IsActive || x.Id == request.SelectedReportTypeDefId))
            .OrderBy(x => x.Name).Select(x => new DefinitionOption(x.Id, x.Code, x.Name)).ToListAsync(ct);
        return new HospitalReportTypeOptionsResponse(hospitals, reports);
    }
}
