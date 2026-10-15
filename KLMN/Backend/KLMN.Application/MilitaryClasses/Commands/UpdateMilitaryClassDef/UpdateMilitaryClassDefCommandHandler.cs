using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.MilitaryClasses.Commands.Common;
using KLMN.Application.MilitaryClasses.Responses;

using MediatR;

namespace KLMN.Application.MilitaryClasses.Commands.UpdateMilitaryClassDef;

/// <summary>Mevcut sınıfı xmin çakışmasını denetleyerek günceller.</summary>
public sealed class UpdateMilitaryClassDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateMilitaryClassDefCommand, MilitaryClassDefMutationResponse>
{
    /// <summary>Sınıf alanlarını mevcut entity üzerinde değiştirir.</summary>
    public async Task<MilitaryClassDefMutationResponse> Handle(UpdateMilitaryClassDefCommand request, CancellationToken ct)
    {
        var item = await MilitaryClassDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await MilitaryClassDefRules.EnsureUniqueCodeAsync(db, request.Code, item.Id, ct);
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ShortName = string.IsNullOrWhiteSpace(request.ShortName) ? null : request.ShortName.Trim();
        item.ExternalCode = string.IsNullOrWhiteSpace(request.ExternalCode) ? null : request.ExternalCode.Trim();
        item.Active = request.Active;
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;

        await db.SaveChangesAsync(ct);
        return new MilitaryClassDefMutationResponse(item.Id, item.Version);
    }
}
