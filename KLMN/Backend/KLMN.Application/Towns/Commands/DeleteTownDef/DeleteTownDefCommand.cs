using MediatR;

namespace KLMN.Application.Towns.Commands.DeleteTownDef;
/// <summary>İlçe tanımı işlem veya doğrulama sözleşmesi; ilgili CQRS katmanında kullanılır.</summary>
public sealed record DeleteTownDefCommand(Guid Id, uint Version) : IRequest;
