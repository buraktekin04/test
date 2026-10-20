using MediatR;
using KLMN.Application.ReasonForExamReportTypes.Responses;
namespace KLMN.Application.ReasonForExamReportTypes.Commands.ChangeStatusReasonForExamReportType;
/// <summary>Muayene Sebebi Rapor Türleri: ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusReasonForExamReportTypeCommand(Guid Id,uint Version,bool IsActive):IRequest<ReasonForExamReportTypeMutationResponse>;
