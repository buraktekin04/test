using FluentValidation;
using KLMN.Domain.Common;

namespace KLMN.Application.MilitaryClassGroups.Commands.CreateMilitaryClassGroup;

/// <summary>Kod, ad, kuvvet kimliği ve UTC geçerlilik döneminin doğrulamasıdır.</summary>
public sealed class CreateMilitaryClassGroupCommandValidator : AbstractValidator<CreateMilitaryClassGroupCommand>
{
    /// <summary>Geçersiz/tarihi ters kaydın handler'a ulaşmasını engeller.</summary>
    public CreateMilitaryClassGroupCommandValidator()
    {

        RuleFor(x => x.ForcesCommandId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ShortName).MaximumLength(100);
        RuleFor(x => x).Must(x => ValidityPeriod.HasValidBounds(x.ValidFrom, x.ValidTo))
            .WithMessage("Geçerli bitiş başlangıçtan sonra olmalıdır.").WithName("ValidTo");
        RuleFor(x => x.ValidFrom).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc);
        RuleFor(x => x.ValidTo).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc);
    }
}
