using MediatR;

namespace KLMN.Application.HospitalReportTypes.Commands.DeleteHospitalReportType;

/// <summary>Sağlık Tesisi Rapor Türü Delete CQRS komutu.</summary>
public sealed record DeleteHospitalReportTypeCommand(Guid Id, uint Version) : IRequest;
