using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Dispatches.Queries.GetDispatches;

/// <summary>Sevk listesini SQL'de filtreler/sayfalar; kimlik numaralarını yalnız maskeli gösterir.</summary>
public sealed class GetDispatchesQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetDispatchesQuery,PagedResult<DispatchListItemResponse>>
{
    public async Task<PagedResult<DispatchListItemResponse>> Handle(GetDispatchesQuery request,CancellationToken ct)
    {
        var q=db.Dispatches.AsNoTracking().Where(x=>!x.IsDeleted &&
            (request.IncludeCancelled||request.CurrentState==4||x.CurrentState!=4));
        if(request.CurrentState.HasValue)
            q=q.Where(x=>x.CurrentState==request.CurrentState.Value);
        if(request.DocumentDateFrom.HasValue)
            q=q.Where(x=>x.DispatchDocumentDate>=request.DocumentDateFrom.Value);
        if(request.DocumentDateTo.HasValue)
            q=q.Where(x=>x.DispatchDocumentDate<=request.DocumentDateTo.Value);
        if(!string.IsNullOrWhiteSpace(request.Search))
        {
            var search=request.Search.Trim().ToUpperInvariant();
            if(long.TryParse(search,out var number))
                q=q.Where(x=>x.DispatchNo==number||x.Person.Name.ToUpper().Contains(search)||x.Person.Surname.ToUpper().Contains(search));
            else
                q=q.Where(x=>x.Person.Name.ToUpper().Contains(search)||x.Person.Surname.ToUpper().Contains(search));
        }
        var total=await q.CountAsync(ct);
        var page=await q.OrderByDescending(x=>x.DispatchNo)
            .Skip((request.PageNumber-1)*request.PageSize).Take(request.PageSize)
            .Select(x=>new {
                x.Id,x.DispatchNo,x.DispatchDate,x.DispatchDocumentDate,x.DispatchDocumentNo,
                x.CurrentState,x.IsActive,x.Version,
                x.Person.Name,x.Person.Surname,x.Person.TcKimlikNo,x.Person.YabanciKimlikNo,
                AdmissionTypeName=db.HcAdmissionTypeDefs.Where(g=>g.Id==x.HcAdmissionTypeDefId).Select(g=>g.Name).FirstOrDefault()
            }).ToListAsync(ct);
        var items=page.Select(x=>new DispatchListItemResponse(x.Id,x.DispatchNo,
            x.Name+" "+x.Surname,Mask(x.TcKimlikNo??x.YabanciKimlikNo),x.DispatchDate,
            x.DispatchDocumentDate,x.DispatchDocumentNo,x.AdmissionTypeName??"",x.CurrentState,x.IsActive,x.Version)).ToList();
        return new PagedResult<DispatchListItemResponse>{Items=items,TotalCount=total,
            PageNumber=request.PageNumber,PageSize=request.PageSize};
    }

    private static string Mask(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "—";
        return new string('*',Math.Max(0,value.Length-4))+value[^Math.Min(4,value.Length)..];
    }
}
