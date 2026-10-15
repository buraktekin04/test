using MediatR;
using KLMN.Application.Countries.Responses;
namespace KLMN.Application.Countries.Commands.CreateCountryDef;
/// <summary>Harici koddan bağımsız ülke ShortCode alanını da içeren oluşturma komutu.</summary>
public sealed record CreateCountryDefCommand(string Code, string Name, string? ExternalCode, bool? IsTownRequired, string? ShortCode = null) : IRequest<CountryDefMutationResponse>;
