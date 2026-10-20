using KLMN.Application.Common.Models;
using KLMN.Application.ReasonForExamBranchDefs.Responses;
using MediatR;
namespace KLMN.Application.ReasonForExamBranchDefs.Queries.GetReasonForExamBranchDefs;
/// <summary>Muayene Gereken Branşlar filtreleri ve backend sayfalama.</summary>
public sealed record GetReasonForExamBranchDefsQuery(string? Search=null,bool IncludeInactive=false,
 int PageNumber=1,int PageSize=20,bool? IsActive=null,Guid? ReasonForExaminationDefId=null)
 :IRequest<PagedResult<ReasonForExamBranchDefResponse>>;
