using MediatR;

namespace KLMN.Application.ReportTypes.Commands.DeleteReportTypeDef;

/// <summary>Rapor Türü Delete CQRS komutu.</summary>
public sealed record DeleteReportTypeDefCommand(Guid Id, uint Version) : IRequest;
