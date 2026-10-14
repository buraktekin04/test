using MediatR;
using KLMN.Application.Cities.Responses;
namespace KLMN.Application.Cities.Commands.CreateCityDef;
public sealed record CreateCityDefCommand(string Code, string Name, string? ExternalCode, Guid? CountryId, int? MernisCode, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<CityDefMutationResponse>;
