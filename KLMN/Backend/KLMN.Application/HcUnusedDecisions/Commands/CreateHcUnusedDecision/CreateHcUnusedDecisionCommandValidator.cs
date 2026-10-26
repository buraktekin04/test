using FluentValidation;
namespace KLMN.Application.HcUnusedDecisions.Commands.CreateHcUnusedDecision;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class CreateHcUnusedDecisionCommandValidator:AbstractValidator<CreateHcUnusedDecisionCommand>
{
 public CreateHcUnusedDecisionCommandValidator(){RuleFor(x=>x.HcDecisionDefId).NotEmpty();RuleFor(x=>x.ReferenceId).NotEmpty();}
}
