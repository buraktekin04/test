using FluentValidation;

namespace KLMN.Application.Dispatches.Commands.CreateBulkDispatch;

/// <summary>Dosyadaki satır sayısını ve temel kişi/sevk sözleşmesini doğrular.</summary>
public sealed class CreateBulkDispatchCommandValidator : AbstractValidator<CreateBulkDispatchCommand>
{
    public CreateBulkDispatchCommandValidator()
    {
        RuleFor(x => x.Items)
            .NotNull()
            .Must(items => items is { Count: >= 1 and <= 100 })
            .WithMessage("Toplu sevk dosyası 1 ile 100 kayıt içermelidir.");

        RuleForEach(x => x.Items).ChildRules(row =>
        {
            row.RuleFor(x => x.Person).NotNull().WithMessage("Kişi bilgisi gereklidir.");
            row.RuleFor(x => x.Dispatch).NotNull().WithMessage("Sevk bilgisi gereklidir.");
        });
    }
}
