using FluentValidation;
namespace KLMN.Application.HcDecisionConnections.Commands.ChangeStatusHcConnectedDecision;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class ChangeStatusHcConnectedDecisionCommandValidator:AbstractValidator<ChangeStatusHcConnectedDecisionCommand>
{
 public ChangeStatusHcConnectedDecisionCommandValidator(){RuleFor(x=>x.Id).NotEmpty();}
}
