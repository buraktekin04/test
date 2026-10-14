using MediatR;
using KLMN.Application.PurchaseNumbers.Responses;

namespace KLMN.Application.PurchaseNumbers.Commands.ChangePurchaseNumberDefStatus;

public sealed record ChangePurchaseNumberDefStatusCommand(Guid Id, uint Version, bool IsActive)
    : IRequest<PurchaseNumberDefMutationResponse>;
