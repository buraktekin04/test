using KLMN.Application.Hospitals.Responses;
using MediatR;

namespace KLMN.Application.Hospitals.Commands.CreateHospitalDef;

/// <summary>Sağlık Tesisi Create CQRS komutu.</summary>
public sealed record CreateHospitalDefCommand(string Code, string Name, Guid? CityId, Guid? RefereeHospitalId, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<HospitalDefMutationResponse>;
