using MediatR;
using KLMN.Application.Countries.Responses;
namespace KLMN.Application.Countries.Commands.UpdateCountryDef;
public sealed record UpdateCountryDefCommand(Guid Id, uint Version, string Code, string Name, string? ExternalCode, bool? IsTownRequired) : IRequest<CountryDefMutationResponse>;
