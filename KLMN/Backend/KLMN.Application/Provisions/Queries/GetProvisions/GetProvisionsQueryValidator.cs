using FluentValidation;
using KLMN.Domain.Entities.Admissions;

namespace KLMN.Application.Provisions.Queries.GetProvisions;

public sealed class GetProvisionsQueryValidator : AbstractValidator<GetProvisionsQuery>
{
    public GetProvisionsQueryValidator()
    {
        RuleFor(x=>x.PageNumber).GreaterThan(0);
        RuleFor(x=>x.PageSize).InclusiveBetween(1,100);
        RuleFor(x=>x.CurrentState).Must(x=>!x.HasValue||
            (x.Value is ProvisionStatusCodes.New or ProvisionStatusCodes.Report or
             ProvisionStatusCodes.Cancelled or ProvisionStatusCodes.Notification));
        RuleFor(x=>x.ProvisionNo).Must(x=>!x.HasValue||x.Value>0);
        RuleFor(x=>x).Must(x=>!x.DateFrom.HasValue||!x.DateTo.HasValue||x.DateTo.Value>=x.DateFrom.Value)
            .WithMessage("Provizyon başlangıç tarihi bitiş tarihinden sonra olamaz.");
        RuleFor(x=>x.TcKimlikNo).MaximumLength(11);
        RuleFor(x=>x.YabanciKimlikNo).MaximumLength(20);
        RuleFor(x=>x.Name).MaximumLength(64);
        RuleFor(x=>x.Surname).MaximumLength(64);
    }
}
