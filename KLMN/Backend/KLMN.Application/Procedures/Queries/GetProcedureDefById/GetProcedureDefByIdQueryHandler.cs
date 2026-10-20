using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Procedures.Exceptions;
using KLMN.Application.Procedures.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Procedures.Queries.GetProcedureDefById;

/// <summary>Kayıt detayını seçilen GUID ile sorgular.</summary>
public sealed class GetProcedureDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetProcedureDefByIdQuery, ProcedureDefResponse>
{
    /// <summary>Silinmiş veya olmayan tetkikı 404 olarak ele alır.</summary>
    public async Task<ProcedureDefResponse> Handle(GetProcedureDefByIdQuery request, CancellationToken ct) =>
        await db.ProcedureDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(ProcedureDefResponse.Projection).SingleOrDefaultAsync(ct)
            ?? throw new ProcedureNotFoundException();
}
