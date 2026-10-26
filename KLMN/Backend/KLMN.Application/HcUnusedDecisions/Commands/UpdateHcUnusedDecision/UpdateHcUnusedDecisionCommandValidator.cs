using FluentValidation;
namespace KLMN.Application.HcUnusedDecisions.Commands.UpdateHcUnusedDecision;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class UpdateHcUnusedDecisionCommandValidator:AbstractValidator<UpdateHcUnusedDecisionCommand>
{
 public UpdateHcUnusedDecisionCommandValidator(){RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.HcDecisionDefId).NotEmpty();RuleFor(x=>x.ReferenceId).NotEmpty();}
}
