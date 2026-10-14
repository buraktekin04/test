using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Towns.Commands.Common;
using KLMN.Application.Towns.Responses;
using KLMN.Domain.Entities.Geography;
using MediatR;
namespace KLMN.Application.Towns.Commands.CreateTownDef;
/// <summary>İlçe tanımı işlem veya doğrulama sözleşmesi; ilgili CQRS katmanında kullanılır.</summary>
public sealed class CreateTownDefCommandHandler(IKLMNDbContext db) : IRequestHandler<CreateTownDefCommand, TownDefMutationResponse>
{
    public async Task<TownDefMutationResponse> Handle(CreateTownDefCommand request, CancellationToken ct)
    {
        var item = new TownDef();
        await TownDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        await TownDefRules.EnsureCityAsync(db, request.CityId, ct);
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ExternalCode = string.IsNullOrWhiteSpace(request.ExternalCode) ? null : request.ExternalCode.Trim();
        item.CityId = request.CityId;
        item.MernisCode = request.MernisCode;
        await db.TownDefs.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new TownDefMutationResponse(item.Id, item.Version);
    }
}
