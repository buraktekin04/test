using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ForcesCommands.Commands.Common;
using KLMN.Application.ForcesCommands.Exceptions;
using KLMN.Application.ForcesCommands.Responses;
using KLMN.Domain.Entities.MilitaryUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.ForcesCommands.Commands.DeleteForcesCommandDef;

/// <summary>Kuvvet komutanlığı Delete işlemini bağımsız CQRS işleyicisiyle yürütür.</summary>
public sealed class DeleteForcesCommandDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteForcesCommandDefCommand>
{
    public async Task Handle(DeleteForcesCommandDefCommand request, CancellationToken ct)
    {
        var force = await ForcesCommandDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (await db.MilitaryUnitDefs.IgnoreQueryFilters().AnyAsync(x => x.ForcesCommandDef == force.Id && !x.IsDeleted, ct))
            throw new ForcesCommandConflictException("Bağlı birlikleri bulunan kuvvet komutanlığı silinemez.");
        await ForcesCommandDefRules.EnsureNoMilitarySchoolsAsync(db, force.Id, deactivate: false, ct);
        await ForcesCommandDefRules.EnsureNoMilitaryClassGroupsAsync(db, force.Id, deactivate: false, ct);
        db.ForcesCommandDefs.Remove(force);
        await db.SaveChangesAsync(ct);
    }
}
