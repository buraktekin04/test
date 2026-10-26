using FluentValidation;
namespace KLMN.Application.HcDecisions.Commands.DeleteHcDecisionDef;
/// <summary>Kurul kararı Delete isteğinin alan doğrulamaları.</summary>
public sealed class DeleteHcDecisionDefCommandValidator:AbstractValidator<DeleteHcDecisionDefCommand>
{
    public DeleteHcDecisionDefCommandValidator()
    {
        RuleFor(x=>x.Id).NotEmpty();
        
    }
}
