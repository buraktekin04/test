using FluentValidation;

namespace KLMN.Application.ForcesCommands.Commands.DeleteForcesCommandDef;

/// <summary>DeleteForcesCommandDef komutuna özgü FluentValidation kurallarıdır.</summary>
public sealed class DeleteForcesCommandDefCommandValidator : AbstractValidator<DeleteForcesCommandDefCommand>
{
    public DeleteForcesCommandDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
