using FluentValidation;

namespace KLMN.Application.Procedures.Queries.GetProcedureDefs;

/// <summary>Başvuru grubu/tanım ekranlarıyla aynı sayfa sınırları.</summary>
public sealed class GetProcedureDefsQueryValidator
    : AbstractValidator<GetProcedureDefsQuery>
{
    /// <summary>Liste/Excel query boyutunu denetler.</summary>
    public GetProcedureDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
