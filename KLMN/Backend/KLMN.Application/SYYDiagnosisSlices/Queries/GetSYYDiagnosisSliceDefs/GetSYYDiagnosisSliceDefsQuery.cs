using KLMN.Application.Common.Models;
using KLMN.Application.SYYDiagnosisSlices.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnosisSlices.Queries.GetSYYDiagnosisSliceDefs;
/// <summary>SYY Dilim Tanımları: filtreli, sayfalı liste.</summary>
public sealed record GetSYYDiagnosisSliceDefsQuery(string? Search=null,bool IncludeInactive=false,int PageNumber=1,int PageSize=20,bool? IsActive=null)
 :IRequest<PagedResult<SYYDiagnosisSliceDefResponse>>;
