using MediatR;
using KLMN.Application.Countries.Responses;
namespace KLMN.Application.Countries.Commands.UpdateCountryDef;
/// <summary>CountryDef kaydını xmin ve bağımsız kısa kod alanıyla günceller.</summary>
public sealed record UpdateCountryDefCommand(Guid Id, uint Version, string Code, string Name, string? ExternalCode, bool? IsTownRequired, string? ShortCode = null) : IRequest<CountryDefMutationResponse>;
