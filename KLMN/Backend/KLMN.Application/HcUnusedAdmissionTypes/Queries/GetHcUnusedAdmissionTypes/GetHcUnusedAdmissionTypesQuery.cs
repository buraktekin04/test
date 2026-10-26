using KLMN.Application.HcUnusedAdmissionTypes.Responses;
using KLMN.Application.Common.Models;
using MediatR;
namespace KLMN.Application.HcUnusedAdmissionTypes.Queries.GetHcUnusedAdmissionTypes;
/// <summary>Kullanılmayacak Başvuru Grupları: sunucu filtresi ve sayfalama sorgusu.</summary>
public sealed record GetHcUnusedAdmissionTypesQuery(string? Search=null,bool IncludeInactive=false,int PageNumber=1,int PageSize=20,bool? IsActive=null,Guid? HcDecisionDefId=null):IRequest<PagedResult<HcUnusedAdmissionTypeResponse>>;
