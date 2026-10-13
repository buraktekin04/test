using FluentValidation;
using KLMN.Domain.Common;

namespace KLMN.Application.MilitaryUnits.Commands.CreateMilitaryUnitDef;

/// <summary>CreateMilitaryUnitDef komutuna özgü FluentValidation kurallarıdır.</summary>
public sealed class CreateMilitaryUnitDefCommandValidator : AbstractValidator<CreateMilitaryUnitDefCommand>
{
    public CreateMilitaryUnitDefCommandValidator()
    {
        RuleFor(x => x.ExternalCode).MaximumLength(100);
        RuleFor(x => x.ForcesCommandDef).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x).Must(x => ValidityPeriod.HasValidBounds(x.ValidFrom, x.ValidTo))
            .WithMessage("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.").WithName("ValidTo");
        RuleFor(x => x.ValidFrom).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Başlangıç tarihi UTC formatında gönderilmelidir.");
        RuleFor(x => x.ValidTo).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Bitiş tarihi UTC formatında gönderilmelidir.");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$").WithMessage("Kod harf, sayı, tire veya alt çizgi içermelidir.");
        RuleFor(x => x.ConnectedTopUnity).Must(x => !x.HasValue || x.Value != Guid.Empty);
    }
}
