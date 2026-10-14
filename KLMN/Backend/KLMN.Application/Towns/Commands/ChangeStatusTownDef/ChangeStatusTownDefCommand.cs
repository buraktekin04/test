using MediatR;
using KLMN.Application.Towns.Responses;
namespace KLMN.Application.Towns.Commands.ChangeStatusTownDef;
/// <summary>İlçe tanımı işlem veya doğrulama sözleşmesi; ilgili CQRS katmanında kullanılır.</summary>
public sealed record ChangeStatusTownDefCommand(Guid Id, uint Version, bool IsActive) : IRequest<TownDefMutationResponse>;
