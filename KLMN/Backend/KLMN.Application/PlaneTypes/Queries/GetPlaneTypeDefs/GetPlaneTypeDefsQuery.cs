using KLMN.Application.Common.Models;
using KLMN.Application.PlaneTypes.Responses;
using MediatR;
namespace KLMN.Application.PlaneTypes.Queries.GetPlaneTypeDefs;
/// <summary>Uçuşa Uçak Tipi Tanımları: filtreli, sayfalı liste.</summary>
public sealed record GetPlaneTypeDefsQuery(string? Search=null,bool IncludeInactive=false,int PageNumber=1,int PageSize=20,bool? IsActive=null)
 :IRequest<PagedResult<PlaneTypeDefResponse>>;
