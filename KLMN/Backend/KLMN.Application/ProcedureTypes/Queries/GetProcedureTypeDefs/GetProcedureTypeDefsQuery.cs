using KLMN.Application.Common.Models;
using KLMN.Application.ProcedureTypes.Responses;
using MediatR;
namespace KLMN.Application.ProcedureTypes.Queries.GetProcedureTypeDefs;
/// <summary>Dinamik Tetkik Türleri backend sayfalama sorgusu.</summary>
public sealed record GetProcedureTypeDefsQuery(string? Search=null,bool IncludeInactive=false,int PageNumber=1,int PageSize=20,bool? IsActive=null):
IRequest<PagedResult<ProcedureTypeDefResponse>>;
