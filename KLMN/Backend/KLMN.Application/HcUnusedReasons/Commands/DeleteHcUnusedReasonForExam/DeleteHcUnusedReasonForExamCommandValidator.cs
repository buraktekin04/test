using FluentValidation;
namespace KLMN.Application.HcUnusedReasons.Commands.DeleteHcUnusedReasonForExam;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class DeleteHcUnusedReasonForExamCommandValidator:AbstractValidator<DeleteHcUnusedReasonForExamCommand>
{
 public DeleteHcUnusedReasonForExamCommandValidator(){RuleFor(x=>x.Id).NotEmpty();}
}
