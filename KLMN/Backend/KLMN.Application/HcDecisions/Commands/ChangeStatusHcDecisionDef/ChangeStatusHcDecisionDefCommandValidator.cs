using FluentValidation;
namespace KLMN.Application.HcDecisions.Commands.ChangeStatusHcDecisionDef;
/// <summary>Kurul kararı ChangeStatus isteğinin alan doğrulamaları.</summary>
public sealed class ChangeStatusHcDecisionDefCommandValidator:AbstractValidator<ChangeStatusHcDecisionDefCommand>
{
    public ChangeStatusHcDecisionDefCommandValidator()
    {
        RuleFor(x=>x.Id).NotEmpty();
        
    }
}
