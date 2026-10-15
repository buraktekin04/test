using FluentValidation;

namespace KLMN.Application.MilitaryClassGroupRelations.Commands.ChangeStatusMilitaryClassGroupRelation;

/// <summary>Durum/silme işlemlerinde Guid ve xmin doğrulaması.</summary>
public sealed class ChangeStatusMilitaryClassGroupRelationCommandValidator : AbstractValidator<ChangeStatusMilitaryClassGroupRelationCommand>
{
    /// <summary>Güncel sürüm zorunludur.</summary>
    public ChangeStatusMilitaryClassGroupRelationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
