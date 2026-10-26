using KLMN.Application.Common.Models;
using KLMN.Application.HcDecisions.Responses;
using MediatR;
namespace KLMN.Application.HcDecisions.Queries.GetHcDecisionDefs;
/// <summary>Kurul kararı arama, durum filtresi ve sayfalama sorgusu.</summary>
public sealed record GetHcDecisionDefsQuery(string? Search=null,bool IncludeInactive=false,int PageNumber=1,int PageSize=20,bool? IsActive=null)
  :IRequest<PagedResult<HcDecisionDefResponse>>;
