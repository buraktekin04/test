using KLMN.Application.PurchaseNumbers.Responses;
using MediatR;

namespace KLMN.Application.PurchaseNumbers.Commands.CreatePurchaseNumberDef;

/// <summary>Bağımsız Create CQRS komutu.</summary>
public sealed record CreatePurchaseNumberDefCommand(string Code, string Name, DateTime? ValidFrom, DateTime? ValidTo, DateTime? HealthReportEndDate, string SupplyDescriptionName)
    : IRequest<PurchaseNumberDefMutationResponse>;
