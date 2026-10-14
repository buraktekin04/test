using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Cities.Commands.Common;
using KLMN.Application.Cities.Responses;
using KLMN.Domain.Entities.Geography;
using MediatR;
namespace KLMN.Application.Cities.Commands.CreateCityDef;
public sealed class CreateCityDefCommandHandler(IKLMNDbContext db) : IRequestHandler<CreateCityDefCommand, CityDefMutationResponse>
{
    public async Task<CityDefMutationResponse> Handle(CreateCityDefCommand request, CancellationToken ct)
    {
        var item = new CityDef();
        await CityDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        await CityDefRules.EnsureCountryAsync(db, request.CountryId, ct);
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ExternalCode = string.IsNullOrWhiteSpace(request.ExternalCode) ? null : request.ExternalCode.Trim();
        item.CountryId = request.CountryId;
        item.MernisCode = request.MernisCode;
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;
        await db.CityDefs.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new CityDefMutationResponse(item.Id, item.Version);
    }
}
