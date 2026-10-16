using FluentValidation;

namespace KLMN.Application.AdmissionTypes.Commands.DeleteHcAdmissionTypeDef;

/// <summary>İşlemde kayıt kimliği ve güncel concurrency sürümü zorunludur.</summary>
public sealed class DeleteHcAdmissionTypeDefCommandValidator
    : AbstractValidator<DeleteHcAdmissionTypeDefCommand>
{
    /// <summary>Boş kimlik/sıfır xmin hatalarını reddeder.</summary>
    public DeleteHcAdmissionTypeDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
