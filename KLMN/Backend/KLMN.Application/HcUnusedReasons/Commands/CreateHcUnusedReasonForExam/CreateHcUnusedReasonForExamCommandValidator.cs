using FluentValidation;
namespace KLMN.Application.HcUnusedReasons.Commands.CreateHcUnusedReasonForExam;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class CreateHcUnusedReasonForExamCommandValidator:AbstractValidator<CreateHcUnusedReasonForExamCommand>
{
 public CreateHcUnusedReasonForExamCommandValidator(){RuleFor(x=>x.HcDecisionDefId).NotEmpty();RuleFor(x=>x.ReferenceId).NotEmpty();}
}
