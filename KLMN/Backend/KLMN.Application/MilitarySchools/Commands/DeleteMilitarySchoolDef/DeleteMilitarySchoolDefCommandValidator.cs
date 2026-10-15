using FluentValidation;

namespace KLMN.Application.MilitarySchools.Commands.DeleteMilitarySchoolDef;

/// <summary>İşlemde kayıt kimliği ve güncel concurrency sürümü zorunludur.</summary>
public sealed class DeleteMilitarySchoolDefCommandValidator : AbstractValidator<DeleteMilitarySchoolDefCommand>
{
    /// <summary>Boş GUID ve sıfır xmin ile değişiklik yapılmasını önler.</summary>
    public DeleteMilitarySchoolDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
