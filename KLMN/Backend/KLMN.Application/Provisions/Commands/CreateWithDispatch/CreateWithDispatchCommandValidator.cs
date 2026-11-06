using FluentValidation;

namespace KLMN.Application.Provisions.Commands.CreateWithDispatch;

public sealed class CreateWithDispatchCommandValidator : AbstractValidator<CreateWithDispatchCommand>
{
    public CreateWithDispatchCommandValidator()
    {
        RuleFor(x=>x).Must(x=>(x.DispatchId.HasValue&&x.DispatchId!=Guid.Empty)==!x.DispatchNo.HasValue)
            .WithMessage("Sevk kaydı veya sevk sıra numarasından yalnızca biri belirtilmelidir.");
        RuleFor(x=>x.DispatchNo).GreaterThan(0).When(x=>x.DispatchNo.HasValue);
        RuleFor(x=>x.HospitalId).NotEmpty().WithMessage("Kabul yapan hastane seçilmelidir.");
    }
}
