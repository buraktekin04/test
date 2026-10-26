using FluentValidation;
namespace KLMN.Application.HcDecisions.Commands.UpdateHcDecisionDef;
/// <summary>Kurul kararı Update isteğinin alan doğrulamaları.</summary>
public sealed class UpdateHcDecisionDefCommandValidator:AbstractValidator<UpdateHcDecisionDefCommand>
{
    public UpdateHcDecisionDefCommandValidator()
    {
        RuleFor(x=>x.Id).NotEmpty();
        RuleFor(x=>x.Code).GreaterThanOrEqualTo(0);
        RuleFor(x=>x.Name).NotEmpty().MaximumLength(500);
        RuleFor(x=>x).Must(x=>!x.ValidFrom.HasValue||!x.ValidTo.HasValue||x.ValidFrom<x.ValidTo).WithMessage("Bitiş tarihi başlangıçtan sonra olmalıdır.");
        RuleFor(x=>x.ValidFrom).Must(x=>!x.HasValue||x.Value.Kind==DateTimeKind.Utc);
        RuleFor(x=>x.ValidTo).Must(x=>!x.HasValue||x.Value.Kind==DateTimeKind.Utc);
    }
}
