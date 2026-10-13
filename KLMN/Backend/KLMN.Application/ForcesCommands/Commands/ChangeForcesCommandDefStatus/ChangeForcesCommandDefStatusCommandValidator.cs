using FluentValidation;

namespace KLMN.Application.ForcesCommands.Commands.ChangeForcesCommandDefStatus;

/// <summary>ChangeForcesCommandDefStatus komutuna özgü FluentValidation kurallarıdır.</summary>
public sealed class ChangeForcesCommandDefStatusCommandValidator : AbstractValidator<ChangeForcesCommandDefStatusCommand>
{
    public ChangeForcesCommandDefStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
