using FluentValidation;

namespace KLMN.Application.MilitaryClassGroups.Commands.ChangeStatusMilitaryClassGroup;

/// <summary>Güncel xmin ve geçerli Id isteyen grup işlem doğrulaması.</summary>
public sealed class ChangeStatusMilitaryClassGroupCommandValidator : AbstractValidator<ChangeStatusMilitaryClassGroupCommand>
{
    /// <summary>Boş kimlik veya sıfır sürüm reddedilir.</summary>
    public ChangeStatusMilitaryClassGroupCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
