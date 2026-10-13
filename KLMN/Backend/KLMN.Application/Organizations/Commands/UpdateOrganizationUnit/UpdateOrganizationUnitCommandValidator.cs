using FluentValidation;

namespace KLMN.Application.Organizations.Commands.UpdateOrganizationUnit;

/// <summary>UpdateOrganizationUnit komutuna özgü FluentValidation kurallarıdır.</summary>
public sealed class UpdateOrganizationUnitCommandValidator : AbstractValidator<UpdateOrganizationUnitCommand>
{
    public UpdateOrganizationUnitCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$").WithMessage("Kod harf, sayı, tire veya alt çizgi içermelidir.");
        RuleFor(x => x.ParentOrganizationUnitId).Must(x => !x.HasValue || x.Value != Guid.Empty);
    }
}
