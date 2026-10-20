using KLMN.Application.Common.Models;
using KLMN.Application.ReasonForExamReportTypes.Responses;
using MediatR;
namespace KLMN.Application.ReasonForExamReportTypes.Queries.GetReasonForExamReportTypes;
/// <summary>Muayene Sebebi Rapor Türleri filtreleri ve backend sayfalama.</summary>
public sealed record GetReasonForExamReportTypesQuery(string? Search=null,bool IncludeInactive=false,
 int PageNumber=1,int PageSize=20,bool? IsActive=null,Guid? ReasonForExaminationDefId=null)
 :IRequest<PagedResult<ReasonForExamReportTypeResponse>>;
