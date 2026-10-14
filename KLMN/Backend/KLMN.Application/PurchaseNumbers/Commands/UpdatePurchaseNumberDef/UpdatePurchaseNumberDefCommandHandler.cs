using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.PurchaseNumbers.Commands.Common;
using KLMN.Application.PurchaseNumbers.Responses;
using MediatR;

namespace KLMN.Application.PurchaseNumbers.Commands.UpdatePurchaseNumberDef;

/// <summary>Alım numarası Update işlemini optimistic concurrency ve soft-delete ile yürütür.</summary>
public sealed class UpdatePurchaseNumberDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdatePurchaseNumberDefCommand, PurchaseNumberDefMutationResponse>
{
    public async Task<PurchaseNumberDefMutationResponse> Handle(UpdatePurchaseNumberDefCommand request, CancellationToken ct)
    {
        var item = await PurchaseNumberDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await PurchaseNumberDefRules.EnsureUniqueCodeAsync(db, request.Code, item.Id, ct);
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.SupplyDescriptionName = request.SupplyDescriptionName.Trim();
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;
        item.HealthReportEndDate = request.HealthReportEndDate;
        await db.SaveChangesAsync(ct);
        return new PurchaseNumberDefMutationResponse(item.Id, item.Version);
    }
}
