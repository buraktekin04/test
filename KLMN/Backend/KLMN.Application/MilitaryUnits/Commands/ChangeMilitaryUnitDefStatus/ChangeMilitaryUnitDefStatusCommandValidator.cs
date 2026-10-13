using FluentValidation;

namespace KLMN.Application.MilitaryUnits.Commands.ChangeMilitaryUnitDefStatus;

/// <summary>ChangeMilitaryUnitDefStatus komutuna özgü FluentValidation kurallarıdır.</summary>
public sealed class ChangeMilitaryUnitDefStatusCommandValidator : AbstractValidator<ChangeMilitaryUnitDefStatusCommand>
{
    public ChangeMilitaryUnitDefStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
