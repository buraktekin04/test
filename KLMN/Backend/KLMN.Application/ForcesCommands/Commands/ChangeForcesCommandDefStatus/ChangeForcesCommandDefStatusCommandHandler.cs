using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ForcesCommands.Commands.Common;
using KLMN.Application.ForcesCommands.Exceptions;
using KLMN.Application.ForcesCommands.Responses;
using KLMN.Domain.Entities.MilitaryUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.ForcesCommands.Commands.ChangeForcesCommandDefStatus;

/// <summary>Kuvvet komutanlığı Change işlemini bağımsız CQRS işleyicisiyle yürütür.</summary>
public sealed class ChangeForcesCommandDefStatusCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeForcesCommandDefStatusCommand, ForcesCommandDefMutationResponse>
{
    public async Task<ForcesCommandDefMutationResponse> Handle(ChangeForcesCommandDefStatusCommand request, CancellationToken ct)
    {
        var force = await ForcesCommandDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (!request.IsActive && await db.MilitaryUnitDefs.IgnoreQueryFilters()
            .AnyAsync(x => x.ForcesCommandDef == force.Id && x.IsActive && !x.IsDeleted, ct))
            throw new ForcesCommandConflictException("Aktif birlikleri bulunan kuvvet komutanlığı pasifleştirilemez.");
        if (!request.IsActive)
        {
            await ForcesCommandDefRules.EnsureNoMilitarySchoolsAsync(db, force.Id, deactivate: true, ct);
            await ForcesCommandDefRules.EnsureNoMilitaryClassGroupsAsync(db, force.Id, deactivate: true, ct);
        }
        force.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new ForcesCommandDefMutationResponse(force.Id, force.Version);
    }
}
