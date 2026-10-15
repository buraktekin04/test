using FluentValidation;

namespace KLMN.Application.MilitaryClassGroupRelations.Commands.DeleteMilitaryClassGroupRelation;

/// <summary>Durum/silme işlemlerinde Guid ve xmin doğrulaması.</summary>
public sealed class DeleteMilitaryClassGroupRelationCommandValidator : AbstractValidator<DeleteMilitaryClassGroupRelationCommand>
{
    /// <summary>Güncel sürüm zorunludur.</summary>
    public DeleteMilitaryClassGroupRelationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
