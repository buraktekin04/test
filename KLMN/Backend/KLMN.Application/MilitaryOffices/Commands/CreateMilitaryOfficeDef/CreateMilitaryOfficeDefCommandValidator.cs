using FluentValidation;
using KLMN.Domain.Common;

namespace KLMN.Application.MilitaryOffices.Commands.CreateMilitaryOfficeDef;

/// <summary>Şube isteği için alan doğrulaması ve xmin sınırları.</summary>
public sealed class CreateMilitaryOfficeDefCommandValidator : AbstractValidator<CreateMilitaryOfficeDefCommand>
{
    /// <summary>HTTP isteğini güvenli sözleşme sınırlarında tutar.</summary>
    public CreateMilitaryOfficeDefCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ExternalCode).MaximumLength(100);
        RuleFor(x => x.CityId).NotNull().WithMessage("Şehir seçimi zorunludur.")
            .Must(x => x.HasValue && x.Value != Guid.Empty).WithMessage("Geçerli bir şehir seçiniz.");
        RuleFor(x => x).Must(x => ValidityPeriod.HasValidBounds(x.ValidFrom, x.ValidTo))
            .WithMessage("Geçerli bitiş başlangıçtan sonra olmalıdır.").WithName("ValidTo");
        RuleFor(x => x.ValidFrom).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Başlangıç tarihi UTC olmalıdır.");
        RuleFor(x => x.ValidTo).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Bitiş tarihi UTC olmalıdır.");
    }
}
