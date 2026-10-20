using MediatR;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Responses;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.ChangeStatusReasonForExamUnusedHcAdmissionType;
/// <summary>Kullanılmayacak Hasta Grupları: ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusReasonForExamUnusedHcAdmissionTypeCommand(Guid Id,uint Version,bool IsActive):IRequest<ReasonForExamUnusedHcAdmissionTypeMutationResponse>;
