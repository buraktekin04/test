using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnoses.Exceptions;
using KLMN.Application.SYYDiagnoses.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnoses.Queries.GetSYYDiagnosisDefById;
/// <summary>Silinmemiş kayıt ve xmin sürümü.</summary>
public sealed class GetSYYDiagnosisDefByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetSYYDiagnosisDefByIdQuery,SYYDiagnosisDefResponse>
{
 public async Task<SYYDiagnosisDefResponse> Handle(GetSYYDiagnosisDefByIdQuery r,CancellationToken ct)=>
 await db.SYYDiagnosisDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>x.Id==r.Id&&!x.IsDeleted)
 .Select(SYYDiagnosisDefResponse.Projection).SingleOrDefaultAsync(ct)??throw new SYYDiagnosisDefNotFoundException();
}
