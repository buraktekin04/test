using FluentValidation;
namespace KLMN.Application.Dispatches.Commands.UpdateDispatch;
/// <summary>Sevk güncelleme komutunun kimlik ve veri denetimi.</summary>
public sealed class UpdateDispatchCommandValidator : AbstractValidator<UpdateDispatchCommand>
{
    public UpdateDispatchCommandValidator()
    {
        RuleFor(x=>x.Id).NotEmpty();
        RuleFor(x=>x.Version).GreaterThan(0U);
        RuleFor(x=>x.Person).NotNull();
        RuleFor(x=>x.Dispatch).NotNull();
        RuleFor(x=>x.Dispatch!.DispatchScope).Must(scope=>scope is 2 or 3)
            .When(x=>x.Dispatch is not null)
            .WithMessage("Sevk kapsamı zorunludur. Şehir veya belirli sağlık tesisi seçiniz.");
        RuleFor(x=>x.Dispatch!.DispatchScopeCityId).NotNull()
            .When(x=>x.Dispatch?.DispatchScope==2)
            .WithMessage("Şehir kapsamı için şehir seçiniz.");
        RuleFor(x=>x.Dispatch!.DispatchScopeHospitalId).NotNull()
            .When(x=>x.Dispatch?.DispatchScope==3)
            .WithMessage("Sağlık tesisi kapsamı için sağlık tesisi seçiniz.");
    }
}
