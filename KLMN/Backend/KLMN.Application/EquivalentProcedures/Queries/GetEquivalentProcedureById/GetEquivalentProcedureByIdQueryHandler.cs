using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.EquivalentProcedures.Exceptions;
using KLMN.Application.EquivalentProcedures.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.EquivalentProcedures.Queries.GetEquivalentProcedureById;
public sealed class GetEquivalentProcedureByIdQueryHandler(IKLMNDbContext db):
 IRequestHandler<GetEquivalentProcedureByIdQuery,EquivalentProcedureResponse>
{
 public async Task<EquivalentProcedureResponse> Handle(GetEquivalentProcedureByIdQuery r,CancellationToken ct)
 =>await db.EquivalentProcedureDefs.IgnoreQueryFilters().AsNoTracking()
   .Where(x=>x.Id==r.Id&&!x.IsDeleted)
   .Select(x=>new EquivalentProcedureResponse(x.Id,x.ProcedureDefId,
    x.Procedure.ProcedureCode,x.Procedure.Name,x.EquivalentProcedureId,
    x.EquivalentProcedure.ProcedureCode,x.EquivalentProcedure.Name,x.IsActive,x.Version,x.CreatedDate))
   .SingleOrDefaultAsync(ct)??throw new EquivalentProcedureNotFoundException();
}
