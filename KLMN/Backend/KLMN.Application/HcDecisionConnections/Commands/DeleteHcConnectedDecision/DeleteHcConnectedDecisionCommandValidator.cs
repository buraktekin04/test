using FluentValidation;
namespace KLMN.Application.HcDecisionConnections.Commands.DeleteHcConnectedDecision;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class DeleteHcConnectedDecisionCommandValidator:AbstractValidator<DeleteHcConnectedDecisionCommand>
{
 public DeleteHcConnectedDecisionCommandValidator(){RuleFor(x=>x.Id).NotEmpty();}
}
