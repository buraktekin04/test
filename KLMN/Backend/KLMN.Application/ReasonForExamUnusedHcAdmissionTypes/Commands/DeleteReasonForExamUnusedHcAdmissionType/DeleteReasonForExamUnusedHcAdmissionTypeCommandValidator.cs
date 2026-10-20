using FluentValidation;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.DeleteReasonForExamUnusedHcAdmissionType;
/// <summary>Kullanılmayacak Hasta Grupları isteği için alan ve sürüm kontrolü.</summary>
public sealed class DeleteReasonForExamUnusedHcAdmissionTypeCommandValidator:AbstractValidator<DeleteReasonForExamUnusedHcAdmissionTypeCommand>
{
 public DeleteReasonForExamUnusedHcAdmissionTypeCommandValidator(){
 
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 }
}
