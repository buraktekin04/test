using FluentValidation;

namespace KLMN.Application.Organizations.Commands.DeleteOrganizationUnit;

/// <summary>DeleteOrganizationUnit komutuna özgü FluentValidation kurallarıdır.</summary>
public sealed class DeleteOrganizationUnitCommandValidator : AbstractValidator<DeleteOrganizationUnitCommand>
{
    public DeleteOrganizationUnitCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
