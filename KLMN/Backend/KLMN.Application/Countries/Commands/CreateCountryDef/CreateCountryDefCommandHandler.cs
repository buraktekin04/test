using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Countries.Commands.Common;
using KLMN.Application.Countries.Responses;
using KLMN.Domain.Entities.Geography;
using MediatR;
namespace KLMN.Application.Countries.Commands.CreateCountryDef;
public sealed class CreateCountryDefCommandHandler(IKLMNDbContext db) : IRequestHandler<CreateCountryDefCommand, CountryDefMutationResponse>
{
    public async Task<CountryDefMutationResponse> Handle(CreateCountryDefCommand request, CancellationToken ct)
    {
        var item = new CountryDef();
        await CountryDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        await CountryDefRules.EnsureUniqueShortCodeAsync(db, request.ShortCode, null, ct);

        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ExternalCode = string.IsNullOrWhiteSpace(request.ExternalCode) ? null : request.ExternalCode.Trim();
        item.ShortCode = string.IsNullOrWhiteSpace(request.ShortCode) ? null : request.ShortCode.Trim().ToUpperInvariant();
        item.IsTownRequired = request.IsTownRequired;
        await db.CountryDefs.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new CountryDefMutationResponse(item.Id, item.Version);
    }
}
