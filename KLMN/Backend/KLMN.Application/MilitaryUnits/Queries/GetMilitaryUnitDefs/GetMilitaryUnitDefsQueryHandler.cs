using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using KLMN.Application.MilitaryUnits.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitDefs;

/// <summary>Aktif/pasif dahil filtreleri tüm veritabanına uygular ve sayfalar.</summary>
public sealed class GetMilitaryUnitDefsQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryUnitDefsQuery, PagedResult<MilitaryUnitDefResponse>>
{
    public async Task<PagedResult<MilitaryUnitDefResponse>> Handle(
        GetMilitaryUnitDefsQuery request, CancellationToken ct)
    {
        var query = request.IncludeInactive || request.IsActive == false
            ? db.MilitaryUnitDefs.IgnoreQueryFilters().Where(x => !x.IsDeleted)
            : db.MilitaryUnitDefs.IgnoreQueryFilters().Where(x => !x.IsDeleted && x.IsActive);

        query = query.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToUpperInvariant();
            query = query.Where(x => x.Code.ToUpper().Contains(search) ||
                x.Name.ToUpper().Contains(search) ||
                (x.ForceCommand != null && (x.ForceCommand.Name.ToUpper().Contains(search) ||
                    x.ForceCommand.Code.ToUpper().Contains(search))) ||
                (x.ExternalCode != null && x.ExternalCode.ToUpper().Contains(search)) ||
                (x.ConnectedTopUnit != null &&
                    (x.ConnectedTopUnit.Name.ToUpper().Contains(search) ||
                     x.ConnectedTopUnit.Code.ToUpper().Contains(search))));
        }
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        if (request.RootOnly == true)
            query = query.Where(x => x.ConnectedTopUnity == null);
        // Kuvvet ve üst birlik FK filtreleri Count/Skip/Take öncesi uygulanır.
        if (request.ForcesCommandDef.HasValue)
            query = query.Where(x => x.ForcesCommandDef == request.ForcesCommandDef.Value);
        if (request.ConnectedTopUnity.HasValue)
            query = query.Where(x => x.ConnectedTopUnity == request.ConnectedTopUnity.Value);

        var count = await query.CountAsync(ct);
        var items = await query
            .OrderBy(x => x.Name).ThenBy(x => x.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(unit =>
        new MilitaryUnitDefResponse
        {
            Id = unit.Id,
            Code = unit.Code,
            ExternalCode = unit.ExternalCode,
            ForcesCommandDef = unit.ForcesCommandDef,
            ForcesCommandName = unit.ForceCommand != null ? unit.ForceCommand.Name : null,
            ForcesCommandCode = unit.ForceCommand != null ? unit.ForceCommand.Code : null,
            ValidFrom = unit.ValidFrom,
            ValidTo = unit.ValidTo,
            Name = unit.Name,
            ConnectedTopUnity = unit.ConnectedTopUnity,
            ConnectedTopUnitName = unit.ConnectedTopUnit != null
                ? unit.ConnectedTopUnit.Name : null,
            ConnectedTopUnitCode = unit.ConnectedTopUnit != null
                ? unit.ConnectedTopUnit.Code : null,
            IsActive = unit.IsActive,
            CreatedDate = unit.CreatedDate,
            Version = unit.Version,
            UserCount = unit.Users.Count(x => !x.IsDeleted),
            ChildCount = unit.Children.Count(x => !x.IsDeleted)
        })
            .ToListAsync(ct);

        return new PagedResult<MilitaryUnitDefResponse>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = count
        };
    }
}
