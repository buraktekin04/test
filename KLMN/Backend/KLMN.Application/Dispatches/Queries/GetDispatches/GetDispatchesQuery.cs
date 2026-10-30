using KLMN.Application.Common.Models;
using KLMN.Application.Dispatches.Responses;
using MediatR;
namespace KLMN.Application.Dispatches.Queries.GetDispatches;
/// <summary>İsme veya sevk sıra numarasına göre yeni sevk kayıtlarını sayfalar.</summary>
public sealed record GetDispatchesQuery(string? Search=null,int PageNumber=1,int PageSize=20,bool IncludeCancelled=false,
    int? CurrentState=null,DateOnly? DocumentDateFrom=null,DateOnly? DocumentDateTo=null)
    : IRequest<PagedResult<DispatchListItemResponse>>;
