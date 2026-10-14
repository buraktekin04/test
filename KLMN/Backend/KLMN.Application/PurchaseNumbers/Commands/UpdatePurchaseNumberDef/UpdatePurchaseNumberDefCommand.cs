using KLMN.Application.PurchaseNumbers.Responses;
using MediatR;

namespace KLMN.Application.PurchaseNumbers.Commands.UpdatePurchaseNumberDef;

/// <summary>Bağımsız Update CQRS komutu.</summary>
public sealed record UpdatePurchaseNumberDefCommand(Guid Id, uint Version, string Code, string Name, DateTime? ValidFrom, DateTime? ValidTo, DateTime? HealthReportEndDate, string SupplyDescriptionName)
    : IRequest<PurchaseNumberDefMutationResponse>;
