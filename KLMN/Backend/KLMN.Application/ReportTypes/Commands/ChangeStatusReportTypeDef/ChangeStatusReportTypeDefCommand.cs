using KLMN.Application.ReportTypes.Responses;
using MediatR;

namespace KLMN.Application.ReportTypes.Commands.ChangeStatusReportTypeDef;

/// <summary>Rapor Türü ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusReportTypeDefCommand(Guid Id, uint Version, bool IsActive) : IRequest<ReportTypeDefMutationResponse>;
