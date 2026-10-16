using KLMN.Application.MilitaryOffices.Responses;
using MediatR;

namespace KLMN.Application.MilitaryOffices.Commands.ChangeStatusMilitaryOfficeDef;

/// <summary>Askerlik şubesi ChangeStatus CQRS isteğidir; xmin işlemlerde korunur.</summary>
public sealed record ChangeStatusMilitaryOfficeDefCommand(Guid Id, uint Version, bool IsActive) : IRequest<MilitaryOfficeDefMutationResponse>;
