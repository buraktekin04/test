using FluentValidation;
using KLMN.Domain.Common;

namespace KLMN.Application.ForcesCommands.Commands.UpdateForcesCommandDef;

/// <summary>UpdateForcesCommandDef komutuna özgü FluentValidation kurallarıdır.</summary>
public sealed class UpdateForcesCommandDefCommandValidator : AbstractValidator<UpdateForcesCommandDefCommand>
{
    public UpdateForcesCommandDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
        RuleFor(x => x).Must(x => ValidityPeriod.HasValidBounds(x.ValidFrom, x.ValidTo))
            .WithMessage("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.").WithName("ValidTo");
        RuleFor(x => x.ValidFrom).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Başlangıç tarihi UTC formatında gönderilmelidir.");
        RuleFor(x => x.ValidTo).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Bitiş tarihi UTC formatında gönderilmelidir.");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$").WithMessage("Kod harf, sayı, tire veya alt çizgi içermelidir.");
    }
}
