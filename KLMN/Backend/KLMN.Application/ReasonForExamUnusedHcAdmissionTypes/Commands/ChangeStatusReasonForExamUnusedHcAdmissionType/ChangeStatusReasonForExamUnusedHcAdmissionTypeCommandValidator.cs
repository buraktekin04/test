using FluentValidation;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.ChangeStatusReasonForExamUnusedHcAdmissionType;
/// <summary>Kullanılmayacak Hasta Grupları isteği için alan ve sürüm kontrolü.</summary>
public sealed class ChangeStatusReasonForExamUnusedHcAdmissionTypeCommandValidator:AbstractValidator<ChangeStatusReasonForExamUnusedHcAdmissionTypeCommand>
{
 public ChangeStatusReasonForExamUnusedHcAdmissionTypeCommandValidator(){
 
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 }
}
