using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.PlaneTypes.Exceptions;
using KLMN.Application.PlaneTypes.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.PlaneTypes.Queries.GetPlaneTypeDefById;
/// <summary>Silinmemiş kayıt ve xmin sürümü.</summary>
public sealed class GetPlaneTypeDefByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetPlaneTypeDefByIdQuery,PlaneTypeDefResponse>
{
 public async Task<PlaneTypeDefResponse> Handle(GetPlaneTypeDefByIdQuery r,CancellationToken ct)=>
 await db.PlaneTypeDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>x.Id==r.Id&&!x.IsDeleted)
 .Select(PlaneTypeDefResponse.Projection).SingleOrDefaultAsync(ct)??throw new PlaneTypeDefNotFoundException();
}
