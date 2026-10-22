using KLMN.Application.Common.Models;
using KLMN.Application.FlyingMissions.Responses;
using MediatR;
namespace KLMN.Application.FlyingMissions.Queries.GetFlyingMissionDefs;
/// <summary>Uçucu Görev Tanımları: filtreli, sayfalı liste.</summary>
public sealed record GetFlyingMissionDefsQuery(string? Search=null,bool IncludeInactive=false,int PageNumber=1,int PageSize=20,bool? IsActive=null)
 :IRequest<PagedResult<FlyingMissionDefResponse>>;
