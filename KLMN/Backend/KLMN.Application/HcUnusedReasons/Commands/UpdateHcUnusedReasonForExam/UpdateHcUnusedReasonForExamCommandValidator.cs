using FluentValidation;
namespace KLMN.Application.HcUnusedReasons.Commands.UpdateHcUnusedReasonForExam;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class UpdateHcUnusedReasonForExamCommandValidator:AbstractValidator<UpdateHcUnusedReasonForExamCommand>
{
 public UpdateHcUnusedReasonForExamCommandValidator(){RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.HcDecisionDefId).NotEmpty();RuleFor(x=>x.ReferenceId).NotEmpty();}
}
