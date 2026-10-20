using MediatR;
using KLMN.Application.ReasonForExamReportTypes.Responses;
namespace KLMN.Application.ReasonForExamReportTypes.Commands.UpdateReasonForExamReportType;
/// <summary>Muayene Sebebi Rapor Türleri: Update CQRS komutu.</summary>
public sealed record UpdateReasonForExamReportTypeCommand(Guid Id,uint Version,Guid ReasonForExaminationDefId,Guid ReferenceId):IRequest<ReasonForExamReportTypeMutationResponse>;
