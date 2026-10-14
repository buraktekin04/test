using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Towns.Commands.Common;

using MediatR;
namespace KLMN.Application.Towns.Commands.DeleteTownDef;
/// <summary>İlçe tanımı işlem veya doğrulama sözleşmesi; ilgili CQRS katmanında kullanılır.</summary>
public sealed class DeleteTownDefCommandHandler(IKLMNDbContext db) : IRequestHandler<DeleteTownDefCommand>
{
    public async Task Handle(DeleteTownDefCommand request, CancellationToken ct)
    {
        var item = await TownDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);

        db.TownDefs.Remove(item);
        await db.SaveChangesAsync(ct);

    }
}
