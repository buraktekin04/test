using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryUnits.Commands.Common;
using KLMN.Application.MilitaryUnits.Responses;
using MediatR;

namespace KLMN.Application.MilitaryUnits.Commands.UpdateMilitaryUnitDef;

/// <summary>Üst birimde döngüye izin vermeden, xmin ile birlik kaydını günceller.</summary>
public sealed class UpdateMilitaryUnitDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateMilitaryUnitDefCommand, MilitaryUnitDefMutationResponse>
{
    public async Task<MilitaryUnitDefMutationResponse> Handle(UpdateMilitaryUnitDefCommand request, CancellationToken ct)
    {
        var unit = await MilitaryUnitDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await MilitaryUnitDefRules.EnsureUniqueCodeAsync(db, request.Code, unit.Id, ct);

        if (unit.ConnectedTopUnity != request.ConnectedTopUnity)
            await MilitaryUnitDefRules.EnsureValidParentAsync(db, request.ConnectedTopUnity, unit.Id, ct);

        unit.ExternalCode = string.IsNullOrWhiteSpace(request.ExternalCode) ? null : request.ExternalCode.Trim();
        unit.ForcesCommandDef = request.ForcesCommandDef;
        unit.ValidFrom = request.ValidFrom;
        unit.ValidTo = request.ValidTo;
        unit.Name = request.Name.Trim();
        unit.Code = request.Code.Trim().ToUpperInvariant();
        unit.ConnectedTopUnity = request.ConnectedTopUnity;

        await db.SaveChangesAsync(ct);
        return new MilitaryUnitDefMutationResponse(unit.Id, unit.Version);
    }
}
