using KLMN.Application.Common.Models;
using KLMN.Application.AnnotationReasons.Responses;
using MediatR;
namespace KLMN.Application.AnnotationReasons.Queries.GetAnnotationReasonDefs;
/// <summary>Zeyil Sebebi Tanımları: filtreli, sayfalı liste.</summary>
public sealed record GetAnnotationReasonDefsQuery(string? Search=null,bool IncludeInactive=false,int PageNumber=1,int PageSize=20,bool? IsActive=null)
 :IRequest<PagedResult<AnnotationReasonDefResponse>>;
