using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.PurchaseNumbers.Commands.Common;
using KLMN.Application.PurchaseNumbers.Responses;
using KLMN.Domain.Entities.PurchaseNumbers;
using MediatR;

namespace KLMN.Application.PurchaseNumbers.Commands.CreatePurchaseNumberDef;

/// <summary>Alım numarası Create işlemini optimistic concurrency ve soft-delete ile yürütür.</summary>
public sealed class CreatePurchaseNumberDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreatePurchaseNumberDefCommand, PurchaseNumberDefMutationResponse>
{
    public async Task<PurchaseNumberDefMutationResponse> Handle(CreatePurchaseNumberDefCommand request, CancellationToken ct)
    {
        await PurchaseNumberDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        var item = new PurchaseNumberDef();
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.SupplyDescriptionName = request.SupplyDescriptionName.Trim();
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;
        item.HealthReportEndDate = request.HealthReportEndDate;
        await db.PurchaseNumberDefs.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new PurchaseNumberDefMutationResponse(item.Id, item.Version);
    }
}
