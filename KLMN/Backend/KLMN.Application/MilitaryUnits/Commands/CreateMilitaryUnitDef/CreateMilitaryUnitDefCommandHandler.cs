using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryUnits.Commands.Common;
using KLMN.Application.MilitaryUnits.Responses;
using KLMN.Domain.Entities.MilitaryUnit;
using MediatR;

namespace KLMN.Application.MilitaryUnits.Commands.CreateMilitaryUnitDef;

/// <summary>Yeni birim kodunu ve üst birim ilişkisinin geçerliliğini doğrulayarak kayıt açar.</summary>
public sealed class CreateMilitaryUnitDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateMilitaryUnitDefCommand, MilitaryUnitDefMutationResponse>
{
    public async Task<MilitaryUnitDefMutationResponse> Handle(CreateMilitaryUnitDefCommand request, CancellationToken ct)
    {
        await MilitaryUnitDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        await MilitaryUnitDefRules.EnsureValidParentAsync(db, request.ConnectedTopUnity, null, ct);

        var unit = new MilitaryUnitDef
        {
            Code = request.Code.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            ExternalCode = string.IsNullOrWhiteSpace(request.ExternalCode) ? null : request.ExternalCode.Trim(),
            ForcesCommandDef = request.ForcesCommandDef,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            ConnectedTopUnity = request.ConnectedTopUnity
        };
        await db.MilitaryUnitDefs.AddAsync(unit, ct);
        await db.SaveChangesAsync(ct);
        return new MilitaryUnitDefMutationResponse(unit.Id, unit.Version);
    }
}
