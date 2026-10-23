using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ForcesCommands.Exceptions;
using KLMN.Application.ForcesCommands.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.ForcesCommands.Queries.GetForcesCommandDefById;

/// <summary>Pasifler dahil soft-delete edilmemiş birimi getirir.</summary>
public sealed class GetForcesCommandDefByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetForcesCommandDefByIdQuery, ForcesCommandDefResponse>
{
    public async Task<ForcesCommandDefResponse> Handle(GetForcesCommandDefByIdQuery request, CancellationToken ct) =>
        await db.ForcesCommandDefs.IgnoreQueryFilters().AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(force =>
        new ForcesCommandDefResponse
        {
            Id = force.Id, Code = force.Code, Name = force.Name,
            ValidFrom = force.ValidFrom, ValidTo = force.ValidTo,
            IsActive = force.IsActive, CreatedDate = force.CreatedDate, Version = force.Version,
            MilitaryUnitCount = force.MilitaryUnits.Count(x => !x.IsDeleted)
        })
            .SingleOrDefaultAsync(ct) ?? throw new ForcesCommandNotFoundException();
}
