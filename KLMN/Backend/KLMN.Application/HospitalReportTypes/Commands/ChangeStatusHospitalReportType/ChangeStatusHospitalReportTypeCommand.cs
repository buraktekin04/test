using KLMN.Application.HospitalReportTypes.Responses;
using MediatR;

namespace KLMN.Application.HospitalReportTypes.Commands.ChangeStatusHospitalReportType;

/// <summary>Sağlık Tesisi Rapor Türü ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusHospitalReportTypeCommand(Guid Id, uint Version, bool IsActive) : IRequest<HospitalReportTypeMutationResponse>;
