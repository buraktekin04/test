using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReportTypes.Exceptions;
using KLMN.Application.ReportTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.ReportTypes.Queries.GetReportTypeDefById;

/// <summary>Kayıt detayını seçilen GUID ile sorgular.</summary>
public sealed class GetReportTypeDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetReportTypeDefByIdQuery, ReportTypeDefResponse>
{
    /// <summary>Silinmiş veya olmayan rapor türüı 404 olarak ele alır.</summary>
    public async Task<ReportTypeDefResponse> Handle(GetReportTypeDefByIdQuery request, CancellationToken ct) =>
        await db.ReportTypeDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(ReportTypeDefResponse.Projection).SingleOrDefaultAsync(ct)
            ?? throw new ReportTypeNotFoundException();
}
