using FluentValidation;

namespace KLMN.Application.Provisions.Commands.CreateWithoutDispatch;

public sealed class CreateWithoutDispatchCommandValidator : AbstractValidator<CreateWithoutDispatchCommand>
{
    public CreateWithoutDispatchCommandValidator()
    {
        RuleFor(x=>x.HospitalId).NotEmpty().WithMessage("Kabul yapan hastane zorunludur.");
        RuleFor(x=>x.Person).NotNull().WithMessage("Kişi bilgileri zorunludur.");
        RuleFor(x=>x.Dispatch).NotNull().WithMessage("Başvuru ve askerî bilgiler zorunludur.");
        RuleFor(x=>x.Dispatch!.HcAdmissionTypeDefId).NotEmpty()
            .When(x=>x.Dispatch is not null).WithMessage("Başvuru grubu seçiniz.");
        RuleFor(x=>x.Dispatch!.ReasonForExaminationDefId).NotEmpty()
            .When(x=>x.Dispatch is not null).WithMessage("Muayene sebebi seçiniz.");
    }
}
