using FluentValidation;

namespace KLMN.Application.AdmissionTypes.Queries.GetHcAdmissionTypeDefs;

/// <summary>Liste ve Excel filtreleri için sunucu sayfalama sınırları.</summary>
public sealed class GetHcAdmissionTypeDefsQueryValidator : AbstractValidator<GetHcAdmissionTypeDefsQuery>
{
    /// <summary>Sayfa başına maksimum 100 kayıt ile güvenli veri sorgusu.</summary>
    public GetHcAdmissionTypeDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
