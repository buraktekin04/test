using KLMN.Application.Hospitals.Responses;
using MediatR;

namespace KLMN.Application.Hospitals.Commands.UpdateHospitalDef;

/// <summary>Sağlık Tesisi Update CQRS komutu.</summary>
public sealed record UpdateHospitalDefCommand(Guid Id, uint Version, string Code, string Name, Guid? CityId, Guid? RefereeHospitalId, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<HospitalDefMutationResponse>;
