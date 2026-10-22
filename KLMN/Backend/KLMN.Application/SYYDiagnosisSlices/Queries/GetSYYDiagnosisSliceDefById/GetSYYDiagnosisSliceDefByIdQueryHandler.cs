using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisSlices.Exceptions;
using KLMN.Application.SYYDiagnosisSlices.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.SYYDiagnosisSlices.Queries.GetSYYDiagnosisSliceDefById;
/// <summary>Silinmemiş kayıt ve xmin sürümü.</summary>
public sealed class GetSYYDiagnosisSliceDefByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetSYYDiagnosisSliceDefByIdQuery,SYYDiagnosisSliceDefResponse>
{
 public async Task<SYYDiagnosisSliceDefResponse> Handle(GetSYYDiagnosisSliceDefByIdQuery r,CancellationToken ct)=>
 await db.SYYDiagnosisSliceDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>x.Id==r.Id&&!x.IsDeleted)
 .Select(SYYDiagnosisSliceDefResponse.Projection).SingleOrDefaultAsync(ct)??throw new SYYDiagnosisSliceDefNotFoundException();
}
