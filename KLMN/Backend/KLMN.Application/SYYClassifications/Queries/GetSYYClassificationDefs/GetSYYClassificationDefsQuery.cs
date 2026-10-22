using KLMN.Application.Common.Models;
using KLMN.Application.SYYClassifications.Responses;
using MediatR;
namespace KLMN.Application.SYYClassifications.Queries.GetSYYClassificationDefs;
/// <summary>Filtrelenebilir SYY Tanı Sınıflandırma Çizelgesi listesi.</summary>
public sealed record GetSYYClassificationDefsQuery(string? Search=null,Guid? SYYDiagnosisDefId=null,
 bool IncludeInactive=false,int PageNumber=1,int PageSize=20,bool? IsActive=null)
 :IRequest<PagedResult<SYYClassificationDefResponse>>;
