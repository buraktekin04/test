using KLMN.Application.Common.Models;
using KLMN.Application.EquivalentProcedures.Responses;
using MediatR;
namespace KLMN.Application.EquivalentProcedures.Queries.GetEquivalentProcedures;
/// <summary>Her iki uçtan tetkik filtresi ile sayfalı liste.</summary>
public sealed record GetEquivalentProceduresQuery(
 string? Search=null,Guid? ProcedureId=null,bool IncludeInactive=false,
 bool? IsActive=null,int PageNumber=1,int PageSize=20)
 :IRequest<PagedResult<EquivalentProcedureResponse>>;
