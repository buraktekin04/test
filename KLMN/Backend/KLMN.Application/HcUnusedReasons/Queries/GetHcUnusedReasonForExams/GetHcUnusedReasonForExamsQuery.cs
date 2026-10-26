using KLMN.Application.HcUnusedReasons.Responses;
using KLMN.Application.Common.Models;
using MediatR;
namespace KLMN.Application.HcUnusedReasons.Queries.GetHcUnusedReasonForExams;
/// <summary>Kullanılmayacak Muayene Sebepleri: sunucu filtresi ve sayfalama sorgusu.</summary>
public sealed record GetHcUnusedReasonForExamsQuery(string? Search=null,bool IncludeInactive=false,int PageNumber=1,int PageSize=20,bool? IsActive=null,Guid? HcDecisionDefId=null):IRequest<PagedResult<HcUnusedReasonForExamResponse>>;
