using FluentValidation;

namespace KLMN.Application.MilitarySchools.Commands.ChangeStatusMilitarySchoolDef;

/// <summary>İşlemde kayıt kimliği ve güncel concurrency sürümü zorunludur.</summary>
public sealed class ChangeStatusMilitarySchoolDefCommandValidator : AbstractValidator<ChangeStatusMilitarySchoolDefCommand>
{
    /// <summary>Boş GUID ve sıfır xmin ile değişiklik yapılmasını önler.</summary>
    public ChangeStatusMilitarySchoolDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
