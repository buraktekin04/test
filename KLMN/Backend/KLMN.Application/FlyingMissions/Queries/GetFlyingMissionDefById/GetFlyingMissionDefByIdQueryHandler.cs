using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.FlyingMissions.Exceptions;
using KLMN.Application.FlyingMissions.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.FlyingMissions.Queries.GetFlyingMissionDefById;
/// <summary>Silinmemiş kayıt ve xmin sürümü.</summary>
public sealed class GetFlyingMissionDefByIdQueryHandler(IKLMNDbContext db):IRequestHandler<GetFlyingMissionDefByIdQuery,FlyingMissionDefResponse>
{
 public async Task<FlyingMissionDefResponse> Handle(GetFlyingMissionDefByIdQuery r,CancellationToken ct)=>
 await db.FlyingMissionDefs.IgnoreQueryFilters().AsNoTracking().Where(x=>x.Id==r.Id&&!x.IsDeleted)
 .Select(x => new FlyingMissionDefResponse(
 x.Id,x.Code,x.Name,x.ValidFrom,x.ValidTo,x.IsActive,x.Version,x.CreatedDate)).SingleOrDefaultAsync(ct)??throw new FlyingMissionDefNotFoundException();
}
