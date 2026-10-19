using FluentValidation;

namespace KLMN.Application.ConnectedDecisions.Commands.ChangeStatusHcConnectedDecisionDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class ChangeStatusHcConnectedDecisionDefCommandValidator : AbstractValidator<ChangeStatusHcConnectedDecisionDefCommand>
{
    /// <summary>Bağlı kurul kararı API sözleşmesine ait validasyon kuralları.</summary>
    public ChangeStatusHcConnectedDecisionDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
