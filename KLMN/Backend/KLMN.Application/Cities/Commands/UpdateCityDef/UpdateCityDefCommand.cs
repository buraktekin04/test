using MediatR;
using KLMN.Application.Cities.Responses;
namespace KLMN.Application.Cities.Commands.UpdateCityDef;
public sealed record UpdateCityDefCommand(Guid Id, uint Version, string Code, string Name, string? ExternalCode, Guid? CountryId, int? MernisCode, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<CityDefMutationResponse>;
