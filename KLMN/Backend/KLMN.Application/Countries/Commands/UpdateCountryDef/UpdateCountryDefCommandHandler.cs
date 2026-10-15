using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Countries.Commands.Common;
using KLMN.Application.Countries.Responses;

using MediatR;
namespace KLMN.Application.Countries.Commands.UpdateCountryDef;
public sealed class UpdateCountryDefCommandHandler(IKLMNDbContext db) : IRequestHandler<UpdateCountryDefCommand, CountryDefMutationResponse>
{
    public async Task<CountryDefMutationResponse> Handle(UpdateCountryDefCommand request, CancellationToken ct)
    {
        var item = await CountryDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await CountryDefRules.EnsureUniqueCodeAsync(db, request.Code, item.Id, ct);
        await CountryDefRules.EnsureUniqueShortCodeAsync(db, request.ShortCode, item.Id, ct);

        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ExternalCode = string.IsNullOrWhiteSpace(request.ExternalCode) ? null : request.ExternalCode.Trim();
        item.ShortCode = string.IsNullOrWhiteSpace(request.ShortCode) ? null : request.ShortCode.Trim().ToUpperInvariant();
        item.IsTownRequired = request.IsTownRequired;

        await db.SaveChangesAsync(ct);
        return new CountryDefMutationResponse(item.Id, item.Version);
    }
}
