using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Towns.Commands.Common;
using KLMN.Application.Towns.Responses;

using MediatR;
namespace KLMN.Application.Towns.Commands.UpdateTownDef;
/// <summary>İlçe tanımı işlem veya doğrulama sözleşmesi; ilgili CQRS katmanında kullanılır.</summary>
public sealed class UpdateTownDefCommandHandler(IKLMNDbContext db) : IRequestHandler<UpdateTownDefCommand, TownDefMutationResponse>
{
    public async Task<TownDefMutationResponse> Handle(UpdateTownDefCommand request, CancellationToken ct)
    {
        var item = await TownDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await TownDefRules.EnsureUniqueCodeAsync(db, request.Code, item.Id, ct);
        await TownDefRules.EnsureCityAsync(db, request.CityId, ct, item.CityId);
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ExternalCode = string.IsNullOrWhiteSpace(request.ExternalCode) ? null : request.ExternalCode.Trim();
        item.CityId = request.CityId;
        item.MernisCode = request.MernisCode;

        await db.SaveChangesAsync(ct);
        return new TownDefMutationResponse(item.Id, item.Version);
    }
}
