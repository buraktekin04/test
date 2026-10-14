using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Cities.Commands.Common;
using KLMN.Application.Cities.Responses;

using MediatR;
namespace KLMN.Application.Cities.Commands.UpdateCityDef;
public sealed class UpdateCityDefCommandHandler(IKLMNDbContext db) : IRequestHandler<UpdateCityDefCommand, CityDefMutationResponse>
{
    public async Task<CityDefMutationResponse> Handle(UpdateCityDefCommand request, CancellationToken ct)
    {
        var item = await CityDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await CityDefRules.EnsureUniqueCodeAsync(db, request.Code, item.Id, ct);
        await CityDefRules.EnsureCountryAsync(db, request.CountryId, ct, item.CountryId);
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ExternalCode = string.IsNullOrWhiteSpace(request.ExternalCode) ? null : request.ExternalCode.Trim();
        item.CountryId = request.CountryId;
        item.MernisCode = request.MernisCode;
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;

        await db.SaveChangesAsync(ct);
        return new CityDefMutationResponse(item.Id, item.Version);
    }
}
