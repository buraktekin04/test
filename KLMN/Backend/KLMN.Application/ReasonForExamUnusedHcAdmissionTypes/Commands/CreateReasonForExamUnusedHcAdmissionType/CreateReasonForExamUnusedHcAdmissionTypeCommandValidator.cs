using FluentValidation;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.CreateReasonForExamUnusedHcAdmissionType;
/// <summary>Kullanılmayacak Hasta Grupları isteği için alan ve sürüm kontrolü.</summary>
public sealed class CreateReasonForExamUnusedHcAdmissionTypeCommandValidator:AbstractValidator<CreateReasonForExamUnusedHcAdmissionTypeCommand>
{
 public CreateReasonForExamUnusedHcAdmissionTypeCommandValidator(){
 RuleFor(x=>x.ReasonForExaminationDefId).NotEmpty();RuleFor(x=>x.ReferenceId).NotEmpty();
 
 
 }
}
