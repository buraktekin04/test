using MediatR;
using KLMN.Application.Countries.Responses;
namespace KLMN.Application.Countries.Queries.GetCountryDefById;
public sealed record GetCountryDefByIdQuery(Guid Id) : IRequest<CountryDefResponse>;
