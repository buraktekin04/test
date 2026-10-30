using FluentValidation;

namespace KLMN.Application.Dispatches.Queries.GetDispatches;

/// <summary>Sevk sorgu filtrelerini ve sayfa sınırlarını doğrular.</summary>
public sealed class GetDispatchesQueryValidator:AbstractValidator<GetDispatchesQuery>
{
    public GetDispatchesQueryValidator()
    {
        RuleFor(x=>x.PageNumber).GreaterThan(0);
        RuleFor(x=>x.PageSize).InclusiveBetween(1,100);
        RuleFor(x=>x.Search).MaximumLength(100);
        RuleFor(x=>x.CurrentState).Must(x=>!x.HasValue||(x.Value>=1&&x.Value<=5))
            .WithMessage("Geçersiz sevk durumu.");
        RuleFor(x=>x.DispatchScope).Must(x=>!x.HasValue||(x.Value>=1&&x.Value<=3))
            .WithMessage("Sevk kapsamı 1-3 arasında olmalıdır.");
        RuleFor(x=>x.DispatchNo).Must(x=>!x.HasValue||x.Value>0);
        RuleFor(x=>x.PurchaseId).Must(x=>!x.HasValue||x.Value>=0);
        RuleFor(x=>x.Name).MaximumLength(64);
        RuleFor(x=>x.Surname).MaximumLength(64);
        RuleFor(x=>x.TcKimlikNo).MaximumLength(11).Matches(@"^[0-9]*$");
        RuleFor(x=>x.YabanciKimlikNo).MaximumLength(20).Matches(@"^[0-9]*$");
        RuleFor(x=>x).Must(x=>!x.DocumentDateFrom.HasValue||!x.DocumentDateTo.HasValue||
            x.DocumentDateFrom.Value<=x.DocumentDateTo.Value)
            .WithMessage("Evrak tarihi başlangıcı, bitiş tarihinden sonra olamaz.");
        RuleFor(x=>x).Must(x=>!x.DateFrom.HasValue||!x.DateTo.HasValue||
            x.DateFrom.Value<=x.DateTo.Value)
            .WithMessage("Sevk başlangıç tarihi, bitiş tarihinden sonra olamaz.");
        RuleFor(x=>x).Must(x=>!x.SenderMilitaryUnitDefId.HasValue||!x.SenderMilitaryOfficeDefId.HasValue)
            .WithMessage("Gönderen makamda yalnızca birlik veya askerlik şubesi seçilebilir.");
        RuleFor(x=>x).Must(x=>!x.IncludeRelatedOrganizations||x.SenderMilitaryUnitDefId.HasValue)
            .WithMessage("Bağlı organizasyonlarda listeleme için gönderen birlik seçiniz.");
        RuleFor(x=>x).Must(x=>!x.DispatchScopeCityId.HasValue||x.DispatchScope==2)
            .WithMessage("Kapsam şehri seçildiğinde sevk kapsamı 'Şehir' olmalıdır.");
        RuleFor(x=>x).Must(x=>!x.DispatchScopeHospitalId.HasValue||x.DispatchScope==3)
            .WithMessage("Kapsam hastanesi seçildiğinde sevk kapsamı 'Hastane' olmalıdır.");
    }
}
