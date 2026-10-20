using KLMN.Application.HospitalReportTypes.Responses;
using MediatR;

namespace KLMN.Application.HospitalReportTypes.Commands.UpdateHospitalReportType;

/// <summary>Sağlık Tesisi Rapor Türü Update CQRS komutu.</summary>
public sealed record UpdateHospitalReportTypeCommand(Guid Id, uint Version, Guid HospitalDefId, Guid ReportTypeDefId) : IRequest<HospitalReportTypeMutationResponse>;
