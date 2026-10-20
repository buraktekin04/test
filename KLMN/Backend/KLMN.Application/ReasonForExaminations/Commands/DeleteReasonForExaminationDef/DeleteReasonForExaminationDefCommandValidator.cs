using FluentValidation;
namespace KLMN.Application.ReasonForExaminations.Commands.DeleteReasonForExaminationDef;
/// <summary>Delete Muayene Sebebi alan ve concurrency doğrulaması.</summary>
public sealed class DeleteReasonForExaminationDefCommandValidator:AbstractValidator<DeleteReasonForExaminationDefCommand>
{
    public DeleteReasonForExaminationDefCommandValidator()
    {
      
      RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
    }
}
