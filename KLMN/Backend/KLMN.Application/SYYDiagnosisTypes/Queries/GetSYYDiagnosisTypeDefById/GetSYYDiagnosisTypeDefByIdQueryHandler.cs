using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisTypes.Exceptions;
using KLMN.Application.SYYDiagnosisTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisTypes.Queries.GetSYYDiagnosisTypeDefById;
/// <summary>Silinmemiş kayıt ve xmin sürümü.</summary>
public sealed class GetSYYDiagnosisTypeDefByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetSYYDiagnosisTypeDefByIdQuery,SYYDiagnosisTypeDefResponse>
{
 public async Task<SYYDiagnosisTypeDefResponse> Handle(GetSYYDiagnosisTypeDefByIdQuery r,CancellationToken ct)=>
 await db.SYYDiagnosisTypeDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>x.Id==r.Id&&!x.IsDeleted)
 .Select(SYYDiagnosisTypeDefResponse.Projection).SingleOrDefaultAsync(ct)??throw new SYYDiagnosisTypeDefNotFoundException();
}
