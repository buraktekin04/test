using FluentValidation;
namespace KLMN.Application.Dispatches.Queries.GetDispatches;
/// <summary>Sorguda ağır ve sınırsız veri erişimini önler.</summary>
public sealed class GetDispatchesQueryValidator:AbstractValidator<GetDispatchesQuery>
{
    public GetDispatchesQueryValidator()
    {
        RuleFor(x=>x.PageNumber).GreaterThan(0);
        RuleFor(x=>x.PageSize).InclusiveBetween(1,100);
        RuleFor(x=>x.Search).MaximumLength(100);
        RuleFor(x=>x.CurrentState).Must(x=>!x.HasValue||(x.Value>=1&&x.Value<=5))
            .WithMessage("Sevk durumu 1-5 arasında olmalıdır.");
        RuleFor(x=>x).Must(x=>!x.DocumentDateFrom.HasValue||!x.DocumentDateTo.HasValue||
            x.DocumentDateFrom.Value<=x.DocumentDateTo.Value)
            .WithMessage("Evrak tarihi başlangıcı, bitiş tarihinden sonra olamaz.");
    }
}
