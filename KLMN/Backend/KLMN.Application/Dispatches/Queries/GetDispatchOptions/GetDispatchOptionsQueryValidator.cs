using FluentValidation;
namespace KLMN.Application.Dispatches.Queries.GetDispatchOptions;
/// <summary>Arama metnini ve desteklenen tanım türlerini sınırlar.</summary>
public sealed class GetDispatchOptionsQueryValidator : AbstractValidator<GetDispatchOptionsQuery>
{
    public GetDispatchOptionsQueryValidator()
    {
        RuleFor(x=>x.Search).MaximumLength(100);
        RuleFor(x=>x.Type).Must(x=>new[]{"countries","towns","cities","hospitals","admissionTypes",
            "reasons","forces","ranks","classes","units","schools","offices","missions","planes"}.Contains(x))
            .WithMessage("Geçersiz sevk tanım türü.");
    }
}
