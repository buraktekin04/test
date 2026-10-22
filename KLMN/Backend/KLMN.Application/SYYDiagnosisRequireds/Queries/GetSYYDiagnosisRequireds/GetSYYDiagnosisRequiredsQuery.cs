using KLMN.Application.Common.Models;
using KLMN.Application.SYYDiagnosisRequireds.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnosisRequireds.Queries.GetSYYDiagnosisRequireds;
public sealed record GetSYYDiagnosisRequiredsQuery(string? Search=null,Guid? SYYDiagnosisDefId=null,bool IncludeInactive=false,
 int PageNumber=1,int PageSize=20,bool? IsActive=null):IRequest<PagedResult<SYYDiagnosisRequiredResponse>>;
