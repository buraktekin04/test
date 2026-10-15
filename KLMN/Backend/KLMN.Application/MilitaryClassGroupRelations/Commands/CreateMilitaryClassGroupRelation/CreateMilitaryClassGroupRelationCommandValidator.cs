using FluentValidation;

namespace KLMN.Application.MilitaryClassGroupRelations.Commands.CreateMilitaryClassGroupRelation;

/// <summary>Üç FK'nin dolu olmasını ve güncellemede xmin sürümünü gerektirir.</summary>
public sealed class CreateMilitaryClassGroupRelationCommandValidator : AbstractValidator<CreateMilitaryClassGroupRelationCommand>
{
    /// <summary>Boş Guid ve eski/boş sürüm reddedilir.</summary>
    public CreateMilitaryClassGroupRelationCommandValidator()
    {

        RuleFor(x => x.ForcesCommandId).NotEmpty();
        RuleFor(x => x.MilitaryClassId).NotEmpty();
        RuleFor(x => x.MilitaryClassGroupId).NotEmpty();
    }
}
