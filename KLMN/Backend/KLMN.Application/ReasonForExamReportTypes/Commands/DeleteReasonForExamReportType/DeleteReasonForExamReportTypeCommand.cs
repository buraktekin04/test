using MediatR;

namespace KLMN.Application.ReasonForExamReportTypes.Commands.DeleteReasonForExamReportType;
/// <summary>Muayene Sebebi Rapor Türleri: Delete CQRS komutu.</summary>
public sealed record DeleteReasonForExamReportTypeCommand(Guid Id,uint Version):IRequest;
