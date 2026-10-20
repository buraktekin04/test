using KLMN.Application.Common.Models;
using KLMN.Application.ReasonForExamProcedureDefs.Responses;
using MediatR;
namespace KLMN.Application.ReasonForExamProcedureDefs.Queries.GetReasonForExamProcedureDefs;
/// <summary>Gereken Tetkikler filtreleri ve backend sayfalama.</summary>
public sealed record GetReasonForExamProcedureDefsQuery(string? Search=null,bool IncludeInactive=false,
 int PageNumber=1,int PageSize=20,bool? IsActive=null,Guid? ReasonForExaminationDefId=null)
 :IRequest<PagedResult<ReasonForExamProcedureDefResponse>>;
