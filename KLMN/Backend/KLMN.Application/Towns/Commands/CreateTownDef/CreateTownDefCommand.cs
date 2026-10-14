using MediatR;
using KLMN.Application.Towns.Responses;
namespace KLMN.Application.Towns.Commands.CreateTownDef;
/// <summary>İlçe tanımı işlem veya doğrulama sözleşmesi; ilgili CQRS katmanında kullanılır.</summary>
public sealed record CreateTownDefCommand(string Code, string Name, string? ExternalCode, Guid? CityId, int? MernisCode) : IRequest<TownDefMutationResponse>;
