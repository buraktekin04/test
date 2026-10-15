using FluentValidation;

namespace KLMN.Application.MilitaryClassGroupRelations.Commands.UpdateMilitaryClassGroupRelation;

/// <summary>Üç FK'nin dolu olmasını ve güncellemede xmin sürümünü gerektirir.</summary>
public sealed class UpdateMilitaryClassGroupRelationCommandValidator : AbstractValidator<UpdateMilitaryClassGroupRelationCommand>
{
    /// <summary>Boş Guid ve eski/boş sürüm reddedilir.</summary>
    public UpdateMilitaryClassGroupRelationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

        RuleFor(x => x.ForcesCommandId).NotEmpty();
        RuleFor(x => x.MilitaryClassId).NotEmpty();
        RuleFor(x => x.MilitaryClassGroupId).NotEmpty();
    }
}
