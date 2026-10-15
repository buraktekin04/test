using FluentValidation;

namespace KLMN.Application.MilitaryClassGroups.Commands.DeleteMilitaryClassGroup;

/// <summary>Güncel xmin ve geçerli Id isteyen grup işlem doğrulaması.</summary>
public sealed class DeleteMilitaryClassGroupCommandValidator : AbstractValidator<DeleteMilitaryClassGroupCommand>
{
    /// <summary>Boş kimlik veya sıfır sürüm reddedilir.</summary>
    public DeleteMilitaryClassGroupCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
