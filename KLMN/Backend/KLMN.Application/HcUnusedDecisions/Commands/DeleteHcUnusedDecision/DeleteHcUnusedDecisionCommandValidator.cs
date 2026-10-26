using FluentValidation;
namespace KLMN.Application.HcUnusedDecisions.Commands.DeleteHcUnusedDecision;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class DeleteHcUnusedDecisionCommandValidator:AbstractValidator<DeleteHcUnusedDecisionCommand>
{
 public DeleteHcUnusedDecisionCommandValidator(){RuleFor(x=>x.Id).NotEmpty();}
}
