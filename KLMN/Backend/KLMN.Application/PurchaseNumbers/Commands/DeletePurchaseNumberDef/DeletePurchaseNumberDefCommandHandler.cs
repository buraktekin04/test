using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.PurchaseNumbers.Commands.Common;
using MediatR;

namespace KLMN.Application.PurchaseNumbers.Commands.DeletePurchaseNumberDef;

public sealed class DeletePurchaseNumberDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeletePurchaseNumberDefCommand>
{
    public async Task Handle(DeletePurchaseNumberDefCommand request, CancellationToken ct)
    {
        var item = await PurchaseNumberDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        db.PurchaseNumberDefs.Remove(item);
        await db.SaveChangesAsync(ct);
    }
}
