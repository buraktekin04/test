using FluentValidation;

namespace KLMN.Application.Users.Agreements.Commands.UploadUserAgreementPdf;

/// <summary>PDF yüklemenin metadata ve uzantı doğrulamasını yapar.</summary>
public sealed class UploadUserAgreementPdfCommandValidator
    : AbstractValidator<UploadUserAgreementPdfCommand>
{
    /// <summary>Kullanıcı kimliği, dosya uzunluğu ve PDF uzantısını doğrular.</summary>
    public UploadUserAgreementPdfCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OriginalFileName)
            .NotEmpty()
            .MaximumLength(255)
            .Must(x => string.Equals(Path.GetExtension(x), ".pdf", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Yalnızca PDF dosyası yüklenebilir.");
        RuleFor(x => x.SizeBytes).GreaterThan(0);
        RuleFor(x => x.Content).NotNull();
    }
}
