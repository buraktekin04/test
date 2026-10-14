using MediatR;
using KLMN.Application.Towns.Responses;
namespace KLMN.Application.Towns.Commands.UpdateTownDef;
/// <summary>İlçe tanımı işlem veya doğrulama sözleşmesi; ilgili CQRS katmanında kullanılır.</summary>
public sealed record UpdateTownDefCommand(Guid Id, uint Version, string Code, string Name, string? ExternalCode, Guid? CityId, int? MernisCode) : IRequest<TownDefMutationResponse>;
