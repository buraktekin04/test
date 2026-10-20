using FluentValidation;
namespace KLMN.Application.ReasonForExaminations.Commands.CreateReasonForExaminationDef;
/// <summary>Create Muayene Sebebi alan ve concurrency doğrulaması.</summary>
public sealed class CreateReasonForExaminationDefCommandValidator:AbstractValidator<CreateReasonForExaminationDefCommand>
{
    public CreateReasonForExaminationDefCommandValidator()
    {
      RuleFor(x=>x.Code).NotEmpty().MaximumLength(100);
      RuleFor(x=>x.Description).NotEmpty().MaximumLength(500);
      RuleFor(x=>x).Must(x=>!x.ValidFrom.HasValue||!x.ValidTo.HasValue||x.ValidFrom<x.ValidTo)
           .WithMessage("Geçerli bitiş başlangıçtan sonra olmalıdır.");
      RuleFor(x=>x.ValidFrom).Must(x=>!x.HasValue||x.Value.Kind==DateTimeKind.Utc);
      RuleFor(x=>x.ValidTo).Must(x=>!x.HasValue||x.Value.Kind==DateTimeKind.Utc);
      
    }
}
