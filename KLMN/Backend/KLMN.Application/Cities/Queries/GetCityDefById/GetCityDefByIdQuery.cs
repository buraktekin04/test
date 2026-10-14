using MediatR;
using KLMN.Application.Cities.Responses;
namespace KLMN.Application.Cities.Queries.GetCityDefById;
public sealed record GetCityDefByIdQuery(Guid Id) : IRequest<CityDefResponse>;
