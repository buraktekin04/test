using KLMN.Application.Common.Models;
using KLMN.Application.SYYDiagnosisProcedures.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnosisProcedures.Queries.GetSYYDiagnosisProcedureDefs;
/// <summary>Filtrelenebilir SYY Gereken Tetkikler listesi.</summary>
public sealed record GetSYYDiagnosisProcedureDefsQuery(string? Search=null,Guid? SYYDiagnosisDefId=null,
 bool IncludeInactive=false,int PageNumber=1,int PageSize=20,bool? IsActive=null)
 :IRequest<PagedResult<SYYDiagnosisProcedureDefResponse>>;
