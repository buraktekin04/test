using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.PurchaseNumbers.Commands.Common;
using KLMN.Application.PurchaseNumbers.Responses;
using MediatR;

namespace KLMN.Application.PurchaseNumbers.Commands.ChangePurchaseNumberDefStatus;

public sealed class ChangePurchaseNumberDefStatusCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangePurchaseNumberDefStatusCommand, PurchaseNumberDefMutationResponse>
{
    public async Task<PurchaseNumberDefMutationResponse> Handle(ChangePurchaseNumberDefStatusCommand request, CancellationToken ct)
    {
        var item = await PurchaseNumberDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new PurchaseNumberDefMutationResponse(item.Id, item.Version);
    }
}
