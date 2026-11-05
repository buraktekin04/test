using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.Provisions.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Provisions.Queries.GetProvisions;

/// <summary>Sayfalama veritabanında; TC ve yabancı kimlik asla düz liste response'unda dönmez.</summary>
public sealed class GetProvisionsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetProvisionsQuery,PagedResult<ProvisionListItemResponse>>
{
    public async Task<PagedResult<ProvisionListItemResponse>> Handle(GetProvisionsQuery request,CancellationToken ct)
    {
        var q=db.Provisions.AsNoTracking().Where(x=>!x.IsDeleted);
        if(request.DateFrom.HasValue)
        {
            var first=request.DateFrom.Value.ToDateTime(TimeOnly.MinValue,DateTimeKind.Utc);
            q=q.Where(x=>x.ProvisionDate>=first);
        }
        if(request.DateTo.HasValue)
        {
            var last=request.DateTo.Value.AddDays(1).ToDateTime(TimeOnly.MinValue,DateTimeKind.Utc);
            q=q.Where(x=>x.ProvisionDate<last);
        }
        if(request.CurrentState.HasValue) q=q.Where(x=>x.CurrentState==request.CurrentState.Value);
        if(request.ProvisionNo.HasValue) q=q.Where(x=>x.ProvisionNo==request.ProvisionNo.Value);
        if(request.HospitalId.HasValue) q=q.Where(x=>x.DispatchHospitalId==request.HospitalId.Value);
        if(!string.IsNullOrWhiteSpace(request.TcKimlikNo))
            q=q.Where(x=>x.Dispatch.Person.TcKimlikNo==request.TcKimlikNo.Trim());
        if(!string.IsNullOrWhiteSpace(request.YabanciKimlikNo))
            q=q.Where(x=>x.Dispatch.Person.YabanciKimlikNo==request.YabanciKimlikNo.Trim());
        if(!string.IsNullOrWhiteSpace(request.Name))
        {
            var name=request.Name.Trim().ToUpper();
            q=q.Where(x=>x.Dispatch.Person.Name.ToUpper().Contains(name));
        }
        if(!string.IsNullOrWhiteSpace(request.Surname))
        {
            var surname=request.Surname.Trim().ToUpper();
            q=q.Where(x=>x.Dispatch.Person.Surname.ToUpper().Contains(surname));
        }
        if(request.PurchaseId.HasValue)
            q=q.Where(x=>x.Dispatch.PurchaseId==request.PurchaseId.Value);

        var total=await q.CountAsync(ct);
        // Kimlik numarasını DB projeksiyonunda alır, API yanıtında yalnız maskeliyi gösterir.
        var rows=await q.OrderByDescending(x=>x.ProvisionNo)
            .Skip((request.PageNumber-1)*request.PageSize).Take(request.PageSize)
            .Select(x=>new
            {
                x.Id,x.ProvisionNo,x.DispatchId,DispatchNo=x.Dispatch.DispatchNo,
                x.ProvisionDate,x.DispatchHospitalId,HospitalName=x.DispatchHospital.Name,
                x.CurrentState,x.WithoutDispatch,x.ObjectionableReportId,
                x.Description,x.Version,
                Name=x.Dispatch.Person.Name,Surname=x.Dispatch.Person.Surname,
                Identity=x.Dispatch.Person.TcKimlikNo??x.Dispatch.Person.YabanciKimlikNo,
                PurchaseId=x.Dispatch.PurchaseId
            }).ToListAsync(ct);
        var items=rows.Select(x=>new ProvisionListItemResponse(
            x.Id,x.ProvisionNo,x.DispatchId,x.DispatchNo,x.ProvisionDate,
            x.DispatchHospitalId,x.HospitalName,x.CurrentState,x.WithoutDispatch,
            x.Name+" "+x.Surname,Mask(x.Identity),x.PurchaseId,
            x.ObjectionableReportId,x.Description,x.Version)).ToList();
        return new PagedResult<ProvisionListItemResponse>
        {
            Items=items,TotalCount=total,PageNumber=request.PageNumber,PageSize=request.PageSize
        };
    }

    private static string Mask(string? value) =>
        string.IsNullOrEmpty(value)?"—":
        value.Length>4 ? new string('*',value.Length-4)+value[^4..]:"****";
}
