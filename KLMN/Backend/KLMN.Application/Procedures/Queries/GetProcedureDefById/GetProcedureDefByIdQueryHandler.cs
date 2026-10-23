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
            .Select(x=> new ProcedureDefResponse
    {
        Id=x.Id,ProcedureCode=x.ProcedureCode,Name=x.Name,RequiresNumericResult=x.RequiresNumericResult,
        RequiresReferenceRange=x.RequiresReferenceRange,RequiresDeviationInfo=x.RequiresDeviationInfo,
        ValidFrom=x.ValidFrom,ValidTo=x.ValidTo,ProcedureType=x.ProcedureType,
        ProcedureTypeName=x.TypeDefinition.Name,IsActive=x.IsActive,CreatedDate=x.CreatedDate,
        CreatedBy=x.CreatedBy,UpdatedDate=x.UpdatedDate,UpdatedBy=x.UpdatedBy,Version=x.Version
    }).SingleOrDefaultAsync(ct)
            ?? throw new ProcedureNotFoundException();
}
