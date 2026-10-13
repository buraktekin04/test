using FluentValidation;

namespace KLMN.Application.MilitaryUnits.Commands.DeleteMilitaryUnitDef;

/// <summary>DeleteMilitaryUnitDef komutuna özgü FluentValidation kurallarıdır.</summary>
public sealed class DeleteMilitaryUnitDefCommandValidator : AbstractValidator<DeleteMilitaryUnitDefCommand>
{
    public DeleteMilitaryUnitDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
