using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HospitalReportTypes.Exceptions;
using KLMN.Application.HospitalReportTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypeById;

/// <summary>Karar detayını seçilen GUID ile sorgular.</summary>
public sealed class GetHospitalReportTypeByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetHospitalReportTypeByIdQuery, HospitalReportTypeResponse>
{
    /// <summary>Silinmiş veya olmayan sağlık tesisi rapor türüı 404 olarak ele alır.</summary>
    public async Task<HospitalReportTypeResponse> Handle(GetHospitalReportTypeByIdQuery request, CancellationToken ct) =>
        await db.HospitalReportTypes.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(HospitalReportTypeResponse.Projection).SingleOrDefaultAsync(ct)
            ?? throw new HospitalReportTypeNotFoundException();
}
