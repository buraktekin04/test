using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ProcedureTypes.Exceptions;
using KLMN.Application.ProcedureTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.ProcedureTypes.Queries.GetProcedureTypeDefById;
/// <summary>Tetkik türü detayını ve xmin değerini getirir.</summary>
public sealed class GetProcedureTypeDefByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetProcedureTypeDefByIdQuery,ProcedureTypeDefResponse>
{
    public async Task<ProcedureTypeDefResponse> Handle(GetProcedureTypeDefByIdQuery r,CancellationToken ct)=>
    await db.ProcedureTypeDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>x.Id==r.Id&&!x.IsDeleted)
      .Select(ProcedureTypeDefResponse.Projection).SingleOrDefaultAsync(ct)??throw new ProcedureTypeNotFoundException();
}
