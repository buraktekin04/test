using KLMN.Application.AdmissionTypes.Responses;
using MediatR;

namespace KLMN.Application.AdmissionTypes.Commands.ChangeStatusHcAdmissionTypeDef;

/// <summary>Başvuru grubu aktif/pasif isteğidir.</summary>
public sealed record ChangeStatusHcAdmissionTypeDefCommand(Guid Id, uint Version, bool IsActive)
    : IRequest<HcAdmissionTypeDefMutationResponse>;
