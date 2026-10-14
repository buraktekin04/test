using FluentValidation;
using KLMN.Domain.Common;

namespace KLMN.Application.PurchaseNumbers.Commands.UpdatePurchaseNumberDef;

public sealed class UpdatePurchaseNumberDefCommandValidator : AbstractValidator<UpdatePurchaseNumberDefCommand>
{
    public UpdatePurchaseNumberDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100)
            .Matches("^[A-Za-z0-9_-]+$").WithMessage("Kod yalnızca harf, sayı, tire ve alt çizgi içermelidir.");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.SupplyDescriptionName).NotEmpty().MaximumLength(500);
        RuleFor(x => x).Must(x => ValidityPeriod.HasValidBounds(x.ValidFrom, x.ValidTo))
            .WithMessage("Bitiş tarihi başlangıçtan sonra olmalıdır.").WithName("ValidTo");
        RuleFor(x => x.ValidFrom).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Başlangıç tarihi UTC olmalıdır.");
        RuleFor(x => x.ValidTo).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Bitiş tarihi UTC olmalıdır.");
        RuleFor(x => x.HealthReportEndDate)
            .Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Sağlık raporu bitiş tarihi UTC formatında olmalıdır.");
    }
}
