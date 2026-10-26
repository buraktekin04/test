using FluentValidation;
namespace KLMN.Application.HcUnusedDecisions.Commands.ChangeStatusHcUnusedDecision;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class ChangeStatusHcUnusedDecisionCommandValidator:AbstractValidator<ChangeStatusHcUnusedDecisionCommand>
{
 public ChangeStatusHcUnusedDecisionCommandValidator(){RuleFor(x=>x.Id).NotEmpty();}
}
