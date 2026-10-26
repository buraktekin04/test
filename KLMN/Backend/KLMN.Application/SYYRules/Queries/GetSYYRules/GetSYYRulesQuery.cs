using KLMN.Application.Common.Models;
using KLMN.Application.SYYRules.Responses;
using MediatR;
namespace KLMN.Application.SYYRules.Queries.GetSYYRules;
/// <summary>SYY Kural Tanımları: yalnız Eligible=true, arama ve sunucu sayfalaması.</summary>
public sealed record GetSYYRulesQuery(string? Search=null,bool IncludeInactive=false,
 int PageNumber=1,int PageSize=20,Guid? SYYDiagnosisDefId=null):IRequest<PagedResult<SYYRuleResponse>>;
