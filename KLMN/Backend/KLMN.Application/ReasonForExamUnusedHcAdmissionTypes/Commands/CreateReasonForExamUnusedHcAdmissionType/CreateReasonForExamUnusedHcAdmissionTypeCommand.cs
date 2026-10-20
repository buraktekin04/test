using MediatR;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Responses;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.CreateReasonForExamUnusedHcAdmissionType;
/// <summary>Kullanılmayacak Hasta Grupları: Create CQRS komutu.</summary>
public sealed record CreateReasonForExamUnusedHcAdmissionTypeCommand(Guid ReasonForExaminationDefId,Guid ReferenceId):IRequest<ReasonForExamUnusedHcAdmissionTypeMutationResponse>;
