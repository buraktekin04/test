using FluentValidation;
namespace KLMN.Application.HcDecisionConnections.Commands.UpdateHcConnectedDecision;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class UpdateHcConnectedDecisionCommandValidator:AbstractValidator<UpdateHcConnectedDecisionCommand>
{
 public UpdateHcConnectedDecisionCommandValidator(){RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.HcDecisionDefId).NotEmpty();RuleFor(x=>x.ReferenceId).NotEmpty();}
}
