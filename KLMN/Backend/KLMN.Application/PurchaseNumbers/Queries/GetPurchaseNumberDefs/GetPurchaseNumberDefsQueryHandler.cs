using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.PurchaseNumbers.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.PurchaseNumbers.Queries.GetPurchaseNumberDefs;

public sealed class GetPurchaseNumberDefsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetPurchaseNumberDefsQuery, PagedResult<PurchaseNumberDefResponse>>
{
    public async Task<PagedResult<PurchaseNumberDefResponse>> Handle(
        GetPurchaseNumberDefsQuery request, CancellationToken ct)
    {
        var query = db.PurchaseNumberDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (request.IncludeInactive || request.IsActive == false || x.IsActive));
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.ToUpper().Contains(search) ||
                x.Name.ToUpper().Contains(search) ||
                x.SupplyDescriptionName.ToUpper().Contains(search));
        }
        if (!string.IsNullOrWhiteSpace(request.SupplyDescription))
        {
            var supply = request.SupplyDescription.Trim().ToUpperInvariant();
            query = query.Where(x => x.SupplyDescriptionName.ToUpper().Contains(supply));
        }
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        var count = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x =>
        new PurchaseNumberDefResponse
        {
            Id = x.Id, Code = x.Code, Name = x.Name,
            SupplyDescriptionName = x.SupplyDescriptionName,
            ValidFrom = x.ValidFrom, ValidTo = x.ValidTo,
            HealthReportEndDate = x.HealthReportEndDate,
            IsActive = x.IsActive, CreatedDate = x.CreatedDate, CreatedBy = x.CreatedBy,
            UpdatedDate = x.UpdatedDate, UpdatedBy = x.UpdatedBy, Version = x.Version
        })
            .ToListAsync(ct);
        return new PagedResult<PurchaseNumberDefResponse>
        {
            Items = items, TotalCount = count,
            PageNumber = request.PageNumber, PageSize = request.PageSize
        };
    }
}
