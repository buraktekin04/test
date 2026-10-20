using MediatR;

namespace KLMN.Application.Hospitals.Commands.DeleteHospitalDef;

/// <summary>Sağlık Tesisi Delete CQRS komutu.</summary>
public sealed record DeleteHospitalDefCommand(Guid Id, uint Version) : IRequest;
