using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ForcesCommands.Commands.Common;
using KLMN.Application.ForcesCommands.Exceptions;
using KLMN.Application.ForcesCommands.Responses;
using KLMN.Domain.Entities.MilitaryUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.ForcesCommands.Commands.UpdateForcesCommandDef;

/// <summary>Kuvvet komutanlığı Update işlemini bağımsız CQRS işleyicisiyle yürütür.</summary>
public sealed class UpdateForcesCommandDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateForcesCommandDefCommand, ForcesCommandDefMutationResponse>
{
    public async Task<ForcesCommandDefMutationResponse> Handle(UpdateForcesCommandDefCommand request, CancellationToken ct)
    {
        var force = await ForcesCommandDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await ForcesCommandDefRules.EnsureUniqueCodeAsync(db, request.Code, force.Id, ct);
        force.Name = request.Name.Trim();
        force.Code = request.Code.Trim().ToUpperInvariant();
        force.ValidFrom = request.ValidFrom;
        force.ValidTo = request.ValidTo;
        await db.SaveChangesAsync(ct);
        return new ForcesCommandDefMutationResponse(force.Id, force.Version);
    }
}
