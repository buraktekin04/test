using KLMN.Application.Common.Models;
using KLMN.Application.ICD10Defs.Responses;
using MediatR;
namespace KLMN.Application.ICD10Defs.Queries.GetICD10Defs;
/// <summary>ICD10 Tanımları: filtre ve sayfalama modeli.</summary>
public sealed record GetICD10DefsQuery(string? Search=null,bool IncludeInactive=false,
 bool? IsActive=null,int PageNumber=1,int PageSize=20)
 :IRequest<PagedResult<ICD10DefResponse>>;
