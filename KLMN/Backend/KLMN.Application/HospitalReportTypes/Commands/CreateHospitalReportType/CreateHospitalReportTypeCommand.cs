using KLMN.Application.HospitalReportTypes.Responses;
using MediatR;

namespace KLMN.Application.HospitalReportTypes.Commands.CreateHospitalReportType;

/// <summary>Sağlık Tesisi Rapor Türü Create CQRS komutu.</summary>
public sealed record CreateHospitalReportTypeCommand(Guid HospitalDefId, Guid ReportTypeDefId) : IRequest<HospitalReportTypeMutationResponse>;
