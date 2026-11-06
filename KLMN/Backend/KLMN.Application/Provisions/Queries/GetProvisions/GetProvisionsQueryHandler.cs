using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Models;
using KLMN.Application.Provisions.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Provisions.Queries.GetProvisions;

/// <summary>Sayfalama veritabanında; TC ve yabancı kimlik asla düz liste response'unda dönmez.</summary>
public sealed class GetProvisionsQueryHandler(IKLMNDbContext db, ICurrentUserService currentUser)
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

        // Eski provizyon sorgusundaki Bağlı Organizasyonları Listele:
        // kullanıcının bağlı olduğu birlik ve tüm alt birlikleri.
        if(request.IncludeRelatedOrganizations)
        {
            var unitId=currentUser.MilitaryUnitDefId;
            if(!unitId.HasValue)
                throw new InvalidDataException("Bağlı organizasyonlarla sorgulama için kullanıcıya birlik atanmalıdır.");
            var units=await db.MilitaryUnitDefs.AsNoTracking()
                .Where(x=>!x.IsDeleted)
                .Select(x=>new {x.Id,x.ConnectedTopUnity}).ToListAsync(ct);
            var byParent=units.Where(x=>x.ConnectedTopUnity.HasValue)
                .GroupBy(x=>x.ConnectedTopUnity!.Value)
                .ToDictionary(x=>x.Key,x=>x.Select(v=>v.Id).ToArray());
            var related=new HashSet<Guid>{unitId.Value};
            var queue=new Queue<Guid>();
            queue.Enqueue(unitId.Value);
            while(queue.Count>0)
            {
                var parent=queue.Dequeue();
                if(!byParent.TryGetValue(parent,out var children))continue;
                foreach(var child in children)
                    if(related.Add(child))queue.Enqueue(child);
            }
            var permitted=related.ToArray();
            q=q.Where(x=>x.Dispatch.SenderMilitaryUnitDefId.HasValue
                && permitted.Contains(x.Dispatch.SenderMilitaryUnitDefId.Value));
        }

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
