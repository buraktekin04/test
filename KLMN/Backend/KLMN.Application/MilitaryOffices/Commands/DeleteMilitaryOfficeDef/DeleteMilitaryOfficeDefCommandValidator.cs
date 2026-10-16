using FluentValidation;

namespace KLMN.Application.MilitaryOffices.Commands.DeleteMilitaryOfficeDef;

/// <summary>Şube isteği için alan doğrulaması ve xmin sınırları.</summary>
public sealed class DeleteMilitaryOfficeDefCommandValidator : AbstractValidator<DeleteMilitaryOfficeDefCommand>
{
    /// <summary>HTTP isteğini güvenli sözleşme sınırlarında tutar.</summary>
    public DeleteMilitaryOfficeDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
