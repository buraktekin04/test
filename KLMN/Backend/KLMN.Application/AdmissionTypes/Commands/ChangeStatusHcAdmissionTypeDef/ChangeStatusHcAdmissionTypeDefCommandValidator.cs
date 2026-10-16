using FluentValidation;

namespace KLMN.Application.AdmissionTypes.Commands.ChangeStatusHcAdmissionTypeDef;

/// <summary>İşlemde kayıt kimliği ve güncel concurrency sürümü zorunludur.</summary>
public sealed class ChangeStatusHcAdmissionTypeDefCommandValidator
    : AbstractValidator<ChangeStatusHcAdmissionTypeDefCommand>
{
    /// <summary>Boş kimlik/sıfır xmin hatalarını reddeder.</summary>
    public ChangeStatusHcAdmissionTypeDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
