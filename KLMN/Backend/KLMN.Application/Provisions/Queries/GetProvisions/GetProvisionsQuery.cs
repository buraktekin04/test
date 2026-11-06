using KLMN.Application.Common.Models;
using KLMN.Application.Provisions.Responses;
using MediatR;

namespace KLMN.Application.Provisions.Queries.GetProvisions;

/// <summary>Eski provizyon sorgusu: tarih, durum, sıra, hastane ve kişi filtresi.</summary>
public sealed record GetProvisionsQuery(
    DateOnly? DateFrom=null,DateOnly? DateTo=null,int? CurrentState=null,
    long? ProvisionNo=null,Guid? HospitalId=null,string? TcKimlikNo=null,
    string? YabanciKimlikNo=null,string? Name=null,string? Surname=null,
    int? PurchaseId=null,int PageNumber=1,int PageSize=20,
    bool IncludeRelatedOrganizations=false)
    : IRequest<PagedResult<ProvisionListItemResponse>>;
