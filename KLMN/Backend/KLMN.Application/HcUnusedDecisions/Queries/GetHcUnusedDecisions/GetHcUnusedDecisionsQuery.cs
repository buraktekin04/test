using KLMN.Application.HcUnusedDecisions.Responses;
using KLMN.Application.Common.Models;
using MediatR;
namespace KLMN.Application.HcUnusedDecisions.Queries.GetHcUnusedDecisions;
/// <summary>Birlikte Verilmeyecek Kararlar: sunucu filtresi ve sayfalama sorgusu.</summary>
public sealed record GetHcUnusedDecisionsQuery(string? Search=null,bool IncludeInactive=false,int PageNumber=1,int PageSize=20,bool? IsActive=null,Guid? HcDecisionDefId=null):IRequest<PagedResult<HcUnusedDecisionResponse>>;
