using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Dispatches.Queries.GetDispatches;

/// <summary>Filtreleri sayfalamadan önce PostgreSQL sorgusunda uygular; kişisel kimlikleri maskeler.</summary>
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

        // Başlangıç/bitiş tarihleri sevkin sisteme kaydedildiği DispatchDate alanındadır (UTC).
        if(request.DateFrom.HasValue)
        {
            var first=DateTime.SpecifyKind(request.DateFrom.Value.ToDateTime(TimeOnly.MinValue),DateTimeKind.Utc);
            q=q.Where(x=>x.DispatchDate>=first);
        }
        if(request.DateTo.HasValue)
        {
            var end=DateTime.SpecifyKind(request.DateTo.Value.ToDateTime(TimeOnly.MaxValue),DateTimeKind.Utc);
            q=q.Where(x=>x.DispatchDate<=end);
        }

        if(request.DispatchNo.HasValue)
            q=q.Where(x=>x.DispatchNo==request.DispatchNo.Value);
        if(request.PurchaseId.HasValue)
            q=q.Where(x=>x.PurchaseId==request.PurchaseId.Value);
        if(!string.IsNullOrWhiteSpace(request.TcKimlikNo))
        {
            var tc=request.TcKimlikNo.Trim();
            q=q.Where(x=>x.Person.TcKimlikNo==tc);
        }
        if(!string.IsNullOrWhiteSpace(request.YabanciKimlikNo))
        {
            var foreign=request.YabanciKimlikNo.Trim();
            q=q.Where(x=>x.Person.YabanciKimlikNo==foreign);
        }
        if(!string.IsNullOrWhiteSpace(request.Name))
        {
            var name=request.Name.Trim().ToUpperInvariant();
            q=q.Where(x=>x.Person.Name.ToUpper().Contains(name));
        }
        if(!string.IsNullOrWhiteSpace(request.Surname))
        {
            var surname=request.Surname.Trim().ToUpperInvariant();
            q=q.Where(x=>x.Person.Surname.ToUpper().Contains(surname));
        }
        if(!string.IsNullOrWhiteSpace(request.Search))
        {
            var search=request.Search.Trim().ToUpperInvariant();
            if(long.TryParse(search,out var number))
                q=q.Where(x=>x.DispatchNo==number||x.Person.Name.ToUpper().Contains(search)||x.Person.Surname.ToUpper().Contains(search));
            else
                q=q.Where(x=>x.Person.Name.ToUpper().Contains(search)||x.Person.Surname.ToUpper().Contains(search));
        }

        if(request.SenderMilitaryUnitDefId.HasValue)
        {
            var senderUnitId=request.SenderMilitaryUnitDefId.Value;
            if(request.IncludeRelatedOrganizations)
            {
                // Birlik hiyerarşisi parent üzerinden genişletilir; döngülerde ziyaret kümesi korur.
                var relations=await db.MilitaryUnitDefs.AsNoTracking()
                    .Where(x=>!x.IsDeleted)
                    .Select(x=>new {x.Id,x.ConnectedTopUnity})
                    .ToListAsync(ct);
                var related=new HashSet<Guid>{senderUnitId};
                var frontier=new Queue<Guid>();
                frontier.Enqueue(senderUnitId);
                while(frontier.Count>0)
                {
                    var parent=frontier.Dequeue();
                    foreach(var child in relations.Where(x=>x.ConnectedTopUnity==parent))
                    {
                        if(related.Add(child.Id))frontier.Enqueue(child.Id);
                    }
                }
                var unitIds=related.ToArray();
                q=q.Where(x=>x.SenderMilitaryUnitDefId.HasValue &&
                    unitIds.Contains(x.SenderMilitaryUnitDefId.Value));
            }
            else
            {
                q=q.Where(x=>x.SenderMilitaryUnitDefId==senderUnitId);
            }
        }
        if(request.SenderMilitaryOfficeDefId.HasValue)
            q=q.Where(x=>x.SenderMilitaryOfficeDefId==request.SenderMilitaryOfficeDefId.Value);

        if(request.DispatchScope.HasValue)
            q=q.Where(x=>x.DispatchScope==request.DispatchScope.Value);
        if(request.DispatchScopeCityId.HasValue)
            q=q.Where(x=>x.DispatchScopeCityId==request.DispatchScopeCityId.Value);
        if(request.DispatchScopeHospitalId.HasValue)
            q=q.Where(x=>x.DispatchScopeHospitalId==request.DispatchScopeHospitalId.Value);

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
        if(string.IsNullOrEmpty(value))return "—";
        return new string('*',Math.Max(0,value.Length-4))+value[^Math.Min(4,value.Length)..];
    }
}
