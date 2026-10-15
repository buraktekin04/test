using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClassGroups.Commands.Common;
using KLMN.Application.MilitaryClassGroups.Responses;
using KLMN.Domain.Entities.MilitaryUnit;
using MediatR;

namespace KLMN.Application.MilitaryClassGroups.Commands.CreateMilitaryClassGroup;

/// <summary>Aktif kuvvet içinde kodu benzersiz yeni grup kaydeder.</summary>
public sealed class CreateMilitaryClassGroupCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateMilitaryClassGroupCommand, MilitaryClassGroupMutationResponse>
{
    /// <summary>Kod/kuvvet/xmin kurallarını denetledikten sonra kaydı saklar.</summary>
    public async Task<MilitaryClassGroupMutationResponse> Handle(CreateMilitaryClassGroupCommand request, CancellationToken ct)
    {
        var item = new MilitaryClassGroupDef();
        await MilitaryClassGroupRules.EnsureForceAsync(db, request.ForcesCommandId, ct);
        await MilitaryClassGroupRules.EnsureUniqueAsync(db, request.ForcesCommandId, request.Code, null, ct);
        item.ForcesCommandId = request.ForcesCommandId;
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ShortName = string.IsNullOrWhiteSpace(request.ShortName) ? null : request.ShortName.Trim();
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;
        await db.MilitaryClassGroupDefs.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new MilitaryClassGroupMutationResponse(item.Id, item.Version);
    }
}
