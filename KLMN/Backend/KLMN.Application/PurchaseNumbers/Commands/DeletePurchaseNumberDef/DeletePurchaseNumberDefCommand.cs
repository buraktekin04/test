using MediatR;

namespace KLMN.Application.PurchaseNumbers.Commands.DeletePurchaseNumberDef;

public sealed record DeletePurchaseNumberDefCommand(Guid Id, uint Version)
    : IRequest;
