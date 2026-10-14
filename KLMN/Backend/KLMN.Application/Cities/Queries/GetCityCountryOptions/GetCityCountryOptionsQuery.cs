using MediatR;
namespace KLMN.Application.Cities.Queries.GetCityCountryOptions;
public sealed record GetCityCountryOptionsQuery(Guid? SelectedCountryId = null)
    : IRequest<IReadOnlyList<CityCountryOptionResponse>>;
public sealed record CityCountryOptionResponse(Guid Id, string Code, string Name);
