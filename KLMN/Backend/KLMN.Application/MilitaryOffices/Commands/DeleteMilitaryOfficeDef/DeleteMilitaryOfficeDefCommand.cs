using MediatR;

namespace KLMN.Application.MilitaryOffices.Commands.DeleteMilitaryOfficeDef;

/// <summary>Askerlik şubesi Delete CQRS isteğidir; xmin işlemlerde korunur.</summary>
public sealed record DeleteMilitaryOfficeDefCommand(Guid Id, uint Version) : IRequest;
