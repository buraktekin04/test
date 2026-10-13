using FluentValidation;

namespace KLMN.Application.Organizations.Commands.CreateOrganizationUnit;

/// <summary>CreateOrganizationUnit komutuna özgü FluentValidation kurallarıdır.</summary>
public sealed class CreateOrganizationUnitCommandValidator : AbstractValidator<CreateOrganizationUnitCommand>
{
    public CreateOrganizationUnitCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$").WithMessage("Kod harf, sayı, tire veya alt çizgi içermelidir.");
        RuleFor(x => x.ParentOrganizationUnitId).Must(x => !x.HasValue || x.Value != Guid.Empty);
    }
}
