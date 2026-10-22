using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisProcedures.Responses;
using KLMN.Application.SYYDiagnosisProcedures.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisProcedures.Queries.GetSYYDiagnosisProcedureDefById;
public sealed class GetSYYDiagnosisProcedureDefByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetSYYDiagnosisProcedureDefByIdQuery,SYYDiagnosisProcedureDefResponse>
{
 public async Task<SYYDiagnosisProcedureDefResponse> Handle(GetSYYDiagnosisProcedureDefByIdQuery r,CancellationToken ct)=>
 await db.SYYDiagnosisProcedureDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>x.Id==r.Id&&!x.IsDeleted)
  .Select(SYYDiagnosisProcedureDefResponse.Projection).SingleOrDefaultAsync(ct)??throw new SYYDiagnosisProcedureDefNotFoundException();
}
