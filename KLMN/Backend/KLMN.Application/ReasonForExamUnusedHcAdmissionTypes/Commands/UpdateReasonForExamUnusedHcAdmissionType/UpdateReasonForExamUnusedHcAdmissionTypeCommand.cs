using MediatR;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Responses;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.UpdateReasonForExamUnusedHcAdmissionType;
/// <summary>Kullanılmayacak Hasta Grupları: Update CQRS komutu.</summary>
public sealed record UpdateReasonForExamUnusedHcAdmissionTypeCommand(Guid Id,uint Version,Guid ReasonForExaminationDefId,Guid ReferenceId):IRequest<ReasonForExamUnusedHcAdmissionTypeMutationResponse>;
