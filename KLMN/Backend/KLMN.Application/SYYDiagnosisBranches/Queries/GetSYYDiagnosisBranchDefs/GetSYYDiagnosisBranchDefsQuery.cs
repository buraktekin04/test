using KLMN.Application.Common.Models;
using KLMN.Application.SYYDiagnosisBranches.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnosisBranches.Queries.GetSYYDiagnosisBranchDefs;
/// <summary>Filtrelenebilir SYY Gereken Branşlar listesi.</summary>
public sealed record GetSYYDiagnosisBranchDefsQuery(string? Search=null,Guid? SYYDiagnosisDefId=null,
 bool IncludeInactive=false,int PageNumber=1,int PageSize=20,bool? IsActive=null)
 :IRequest<PagedResult<SYYDiagnosisBranchDefResponse>>;
