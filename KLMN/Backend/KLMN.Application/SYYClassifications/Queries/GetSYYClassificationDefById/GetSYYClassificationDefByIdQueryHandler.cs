using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYClassifications.Responses;
using KLMN.Application.SYYClassifications.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYClassifications.Queries.GetSYYClassificationDefById;
public sealed class GetSYYClassificationDefByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetSYYClassificationDefByIdQuery,SYYClassificationDefResponse>
{
 public async Task<SYYClassificationDefResponse> Handle(GetSYYClassificationDefByIdQuery r,CancellationToken ct)=>
 await db.SYYClassificationDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>x.Id==r.Id&&!x.IsDeleted)
  .Select(x=>new SYYClassificationDefResponse(x.Id,x.SYYDiagnosisDefId,x.ForcesCommandDef,x.RankGroup,x.MilitaryClassGroup,x.HcDecision,x.Diagnosis.Name,x.ForcesCommand.Name,x.RankGroupDefinition.Name,x.MilitaryClassGroupDefinition.Name,x.Decision.Name,x.ValidFrom,x.ValidTo,x.IsActive,x.Version,x.CreatedDate)).SingleOrDefaultAsync(ct)??throw new SYYClassificationDefNotFoundException();
}
