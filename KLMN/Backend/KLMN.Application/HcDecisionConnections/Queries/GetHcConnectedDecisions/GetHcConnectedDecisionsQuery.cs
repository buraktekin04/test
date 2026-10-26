using KLMN.Application.HcDecisionConnections.Responses;
using KLMN.Application.Common.Models;
using MediatR;
namespace KLMN.Application.HcDecisionConnections.Queries.GetHcConnectedDecisions;
/// <summary>Bağlı Kararlar: sunucu filtresi ve sayfalama sorgusu.</summary>
public sealed record GetHcConnectedDecisionsQuery(string? Search=null,bool IncludeInactive=false,int PageNumber=1,int PageSize=20,bool? IsActive=null,Guid? HcDecisionDefId=null):IRequest<PagedResult<HcConnectedDecisionResponse>>;
