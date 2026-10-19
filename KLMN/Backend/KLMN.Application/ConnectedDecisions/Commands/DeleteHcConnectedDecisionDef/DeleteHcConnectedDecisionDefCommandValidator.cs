using FluentValidation;

namespace KLMN.Application.ConnectedDecisions.Commands.DeleteHcConnectedDecisionDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class DeleteHcConnectedDecisionDefCommandValidator : AbstractValidator<DeleteHcConnectedDecisionDefCommand>
{
    /// <summary>Bağlı kurul kararı API sözleşmesine ait validasyon kuralları.</summary>
    public DeleteHcConnectedDecisionDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
