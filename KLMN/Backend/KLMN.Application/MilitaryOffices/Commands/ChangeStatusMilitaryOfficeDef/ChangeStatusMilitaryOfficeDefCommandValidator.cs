using FluentValidation;

namespace KLMN.Application.MilitaryOffices.Commands.ChangeStatusMilitaryOfficeDef;

/// <summary>Şube isteği için alan doğrulaması ve xmin sınırları.</summary>
public sealed class ChangeStatusMilitaryOfficeDefCommandValidator : AbstractValidator<ChangeStatusMilitaryOfficeDefCommand>
{
    /// <summary>HTTP isteğini güvenli sözleşme sınırlarında tutar.</summary>
    public ChangeStatusMilitaryOfficeDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
