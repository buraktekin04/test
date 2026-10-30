using KLMN.Application.Common.Models;
using KLMN.Application.Dispatches.Responses;
using MediatR;

namespace KLMN.Application.Dispatches.Queries.GetDispatches;

/// <summary>Sevk listesinde sunucu taraflı ayrıntılı filtreler ve sayfalama.</summary>
public sealed record GetDispatchesQuery(
    string? Search=null,int PageNumber=1,int PageSize=20,bool IncludeCancelled=false,
    int? CurrentState=null,DateOnly? DocumentDateFrom=null,DateOnly? DocumentDateTo=null,
    DateOnly? DateFrom=null,DateOnly? DateTo=null,
    long? DispatchNo=null,int? PurchaseId=null,
    string? TcKimlikNo=null,string? YabanciKimlikNo=null,
    string? Name=null,string? Surname=null,
    Guid? SenderMilitaryUnitDefId=null,Guid? SenderMilitaryOfficeDefId=null,
    bool IncludeRelatedOrganizations=false,
    int? DispatchScope=null,Guid? DispatchScopeCityId=null,Guid? DispatchScopeHospitalId=null)
    : IRequest<PagedResult<DispatchListItemResponse>>;
