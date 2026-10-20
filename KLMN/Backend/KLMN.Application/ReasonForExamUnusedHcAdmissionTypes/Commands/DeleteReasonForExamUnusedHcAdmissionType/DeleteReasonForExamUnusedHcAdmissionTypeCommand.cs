using MediatR;

namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.DeleteReasonForExamUnusedHcAdmissionType;
/// <summary>Kullanılmayacak Hasta Grupları: Delete CQRS komutu.</summary>
public sealed record DeleteReasonForExamUnusedHcAdmissionTypeCommand(Guid Id,uint Version):IRequest;
