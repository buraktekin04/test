using FluentValidation;
namespace KLMN.Application.HcUnusedReasons.Commands.ChangeStatusHcUnusedReasonForExam;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class ChangeStatusHcUnusedReasonForExamCommandValidator:AbstractValidator<ChangeStatusHcUnusedReasonForExamCommand>
{
 public ChangeStatusHcUnusedReasonForExamCommandValidator(){RuleFor(x=>x.Id).NotEmpty();}
}
