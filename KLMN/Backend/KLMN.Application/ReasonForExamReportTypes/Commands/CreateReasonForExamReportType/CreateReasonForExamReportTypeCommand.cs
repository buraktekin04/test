using MediatR;
using KLMN.Application.ReasonForExamReportTypes.Responses;
namespace KLMN.Application.ReasonForExamReportTypes.Commands.CreateReasonForExamReportType;
/// <summary>Muayene Sebebi Rapor Türleri: Create CQRS komutu.</summary>
public sealed record CreateReasonForExamReportTypeCommand(Guid ReasonForExaminationDefId,Guid ReferenceId):IRequest<ReasonForExamReportTypeMutationResponse>;
