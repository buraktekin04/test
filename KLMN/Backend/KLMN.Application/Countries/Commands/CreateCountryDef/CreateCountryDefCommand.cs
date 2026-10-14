using MediatR;
using KLMN.Application.Countries.Responses;
namespace KLMN.Application.Countries.Commands.CreateCountryDef;
public sealed record CreateCountryDefCommand(string Code, string Name, string? ExternalCode, bool? IsTownRequired) : IRequest<CountryDefMutationResponse>;
