using KLMN.Application.Common.Models;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Responses;
using MediatR;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Queries.GetReasonForExamUnusedHcAdmissionTypes;
/// <summary>Kullanılmayacak Hasta Grupları filtreleri ve backend sayfalama.</summary>
public sealed record GetReasonForExamUnusedHcAdmissionTypesQuery(string? Search=null,bool IncludeInactive=false,
 int PageNumber=1,int PageSize=20,bool? IsActive=null,Guid? ReasonForExaminationDefId=null)
 :IRequest<PagedResult<ReasonForExamUnusedHcAdmissionTypeResponse>>;
