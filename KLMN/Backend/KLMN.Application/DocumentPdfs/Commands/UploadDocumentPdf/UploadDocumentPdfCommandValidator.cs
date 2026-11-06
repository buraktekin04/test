using FluentValidation;

namespace KLMN.Application.DocumentPdfs.Commands.UploadDocumentPdf;

/// <summary>Dosya adı, boyut ve uzantı için iş doğrulaması.</summary>
public sealed class UploadDocumentPdfCommandValidator : AbstractValidator<UploadDocumentPdfCommand>
{
    public UploadDocumentPdfCommandValidator()
    {
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255)
            .Must(name => string.Equals(Path.GetExtension(name), ".pdf", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Yalnızca .pdf uzantılı dosya yüklenebilir.");
        RuleFor(x => x.SizeBytes).GreaterThan(0);
        RuleFor(x => x.Content).NotNull();
    }
}

