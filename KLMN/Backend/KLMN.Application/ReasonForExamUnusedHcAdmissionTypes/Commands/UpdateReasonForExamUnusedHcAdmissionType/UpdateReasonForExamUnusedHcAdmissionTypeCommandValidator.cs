using FluentValidation;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.UpdateReasonForExamUnusedHcAdmissionType;
/// <summary>Kullanılmayacak Hasta Grupları isteği için alan ve sürüm kontrolü.</summary>
public sealed class UpdateReasonForExamUnusedHcAdmissionTypeCommandValidator:AbstractValidator<UpdateReasonForExamUnusedHcAdmissionTypeCommand>
{
 public UpdateReasonForExamUnusedHcAdmissionTypeCommandValidator(){
 RuleFor(x=>x.ReasonForExaminationDefId).NotEmpty();RuleFor(x=>x.ReferenceId).NotEmpty();
 
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 }
}
