using KLMN.Application.Common.Models;
using KLMN.Application.SYYDiagnoses.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnoses.Queries.GetSYYDiagnosisDefs;
/// <summary>SYY Tanı Listesi (M/D/F): filtreli, sayfalı liste.</summary>
public sealed record GetSYYDiagnosisDefsQuery(string? Search=null,bool IncludeInactive=false,int PageNumber=1,int PageSize=20,bool? IsActive=null)
 :IRequest<PagedResult<SYYDiagnosisDefResponse>>;
