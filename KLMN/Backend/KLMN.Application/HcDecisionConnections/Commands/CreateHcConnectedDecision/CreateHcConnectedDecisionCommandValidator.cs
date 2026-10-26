using FluentValidation;
namespace KLMN.Application.HcDecisionConnections.Commands.CreateHcConnectedDecision;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class CreateHcConnectedDecisionCommandValidator:AbstractValidator<CreateHcConnectedDecisionCommand>
{
 public CreateHcConnectedDecisionCommandValidator(){RuleFor(x=>x.HcDecisionDefId).NotEmpty();RuleFor(x=>x.ReferenceId).NotEmpty();}
}
