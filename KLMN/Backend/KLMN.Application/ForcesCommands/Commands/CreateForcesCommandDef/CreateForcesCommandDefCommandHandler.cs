using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ForcesCommands.Commands.Common;
using KLMN.Application.ForcesCommands.Exceptions;
using KLMN.Application.ForcesCommands.Responses;
using KLMN.Domain.Entities.MilitaryUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.ForcesCommands.Commands.CreateForcesCommandDef;

/// <summary>Kuvvet komutanlığı Create işlemini bağımsız CQRS işleyicisiyle yürütür.</summary>
public sealed class CreateForcesCommandDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateForcesCommandDefCommand, ForcesCommandDefMutationResponse>
{
    public async Task<ForcesCommandDefMutationResponse> Handle(CreateForcesCommandDefCommand request, CancellationToken ct)
    {
        await ForcesCommandDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        var force = new ForcesCommandDef();
        force.Name = request.Name.Trim();
        force.Code = request.Code.Trim().ToUpperInvariant();
        force.ValidFrom = request.ValidFrom;
        force.ValidTo = request.ValidTo;
        await db.ForcesCommandDefs.AddAsync(force, ct);
        await db.SaveChangesAsync(ct);
        return new ForcesCommandDefMutationResponse(force.Id, force.Version);
    }
}
