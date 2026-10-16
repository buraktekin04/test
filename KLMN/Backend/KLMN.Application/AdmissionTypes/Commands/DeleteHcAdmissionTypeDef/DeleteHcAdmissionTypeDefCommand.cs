using MediatR;

namespace KLMN.Application.AdmissionTypes.Commands.DeleteHcAdmissionTypeDef;

/// <summary>Başvuru grubu soft-delete isteğidir.</summary>
public sealed record DeleteHcAdmissionTypeDefCommand(Guid Id, uint Version)
    : IRequest;
