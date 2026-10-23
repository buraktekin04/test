using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryUnits.Exceptions;
using KLMN.Application.MilitaryUnits.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitDefById;

/// <summary>Pasifler dahil soft-delete edilmemiş birimi getirir.</summary>
public sealed class GetMilitaryUnitDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetMilitaryUnitDefByIdQuery, MilitaryUnitDefResponse>
{
    public async Task<MilitaryUnitDefResponse> Handle(GetMilitaryUnitDefByIdQuery request, CancellationToken ct) =>
        await db.MilitaryUnitDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
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
            .SingleOrDefaultAsync(ct) ?? throw new MilitaryUnitNotFoundException();
}
