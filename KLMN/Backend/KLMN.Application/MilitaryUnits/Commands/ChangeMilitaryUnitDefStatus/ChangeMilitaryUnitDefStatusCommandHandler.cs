using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryUnits.Commands.Common;
using KLMN.Application.MilitaryUnits.Exceptions;
using KLMN.Application.MilitaryUnits.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.MilitaryUnits.Commands.ChangeMilitaryUnitDefStatus;

/// <summary>Bağlı aktif kullanıcı veya alt birim varsa pasifleştirmeyi engeller.</summary>
public sealed class ChangeMilitaryUnitDefStatusCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeMilitaryUnitDefStatusCommand, MilitaryUnitDefMutationResponse>
{
    public async Task<MilitaryUnitDefMutationResponse> Handle(ChangeMilitaryUnitDefStatusCommand request, CancellationToken ct)
    {
        var unit = await MilitaryUnitDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (!request.IsActive)
        {
            var activeUsers = await db.Users.IgnoreQueryFilters()
                .AnyAsync(x => x.MilitaryUnitDefId == unit.Id && !x.IsDeleted && x.IsActive, ct);
            var activeChildren = await db.MilitaryUnitDefs.IgnoreQueryFilters()
                .AnyAsync(x => x.ConnectedTopUnity == unit.Id && !x.IsDeleted && x.IsActive, ct);
            if (activeUsers || activeChildren)
                throw new MilitaryUnitConflictException(
                    "Aktif kullanıcı veya aktif alt birim bulunan birlik pasifleştirilemez.");
        }
        else if (unit.ConnectedTopUnity.HasValue)
        {
            await MilitaryUnitDefRules.EnsureValidParentAsync(db, unit.ConnectedTopUnity, unit.Id, ct);
        }

        unit.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new MilitaryUnitDefMutationResponse(unit.Id, unit.Version);
    }
}
