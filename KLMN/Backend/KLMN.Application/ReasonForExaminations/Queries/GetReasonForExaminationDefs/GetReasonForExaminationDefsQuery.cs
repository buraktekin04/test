using KLMN.Application.Common.Models;
using KLMN.Application.ReasonForExaminations.Responses;
using MediatR;
namespace KLMN.Application.ReasonForExaminations.Queries.GetReasonForExaminationDefs;
/// <summary>Muayene sebebi arama, durum filtresi ve sayfalama.</summary>
public sealed record GetReasonForExaminationDefsQuery(string? Search=null,bool IncludeInactive=false,
    int PageNumber=1,int PageSize=20,bool? IsActive=null):IRequest<PagedResult<ReasonForExaminationDefResponse>>;
