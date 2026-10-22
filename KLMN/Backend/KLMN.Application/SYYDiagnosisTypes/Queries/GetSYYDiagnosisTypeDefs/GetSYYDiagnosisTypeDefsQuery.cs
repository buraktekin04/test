using KLMN.Application.Common.Models;
using KLMN.Application.SYYDiagnosisTypes.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnosisTypes.Queries.GetSYYDiagnosisTypeDefs;
/// <summary>SYY Tanı Türü Tanımları: filtreli, sayfalı liste.</summary>
public sealed record GetSYYDiagnosisTypeDefsQuery(string? Search=null,bool IncludeInactive=false,int PageNumber=1,int PageSize=20,bool? IsActive=null)
 :IRequest<PagedResult<SYYDiagnosisTypeDefResponse>>;
