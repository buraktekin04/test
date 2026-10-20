using KLMN.Application.Hospitals.Responses;
using MediatR;

namespace KLMN.Application.Hospitals.Commands.ChangeStatusHospitalDef;

/// <summary>Sağlık Tesisi ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusHospitalDefCommand(Guid Id, uint Version, bool IsActive) : IRequest<HospitalDefMutationResponse>;
