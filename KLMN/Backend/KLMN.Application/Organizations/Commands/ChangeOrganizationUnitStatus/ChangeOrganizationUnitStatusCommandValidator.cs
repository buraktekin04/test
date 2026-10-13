using FluentValidation;

namespace KLMN.Application.Organizations.Commands.ChangeOrganizationUnitStatus;

/// <summary>ChangeOrganizationUnitStatus komutuna özgü FluentValidation kurallarıdır.</summary>
public sealed class ChangeOrganizationUnitStatusCommandValidator : AbstractValidator<ChangeOrganizationUnitStatusCommand>
{
    public ChangeOrganizationUnitStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
