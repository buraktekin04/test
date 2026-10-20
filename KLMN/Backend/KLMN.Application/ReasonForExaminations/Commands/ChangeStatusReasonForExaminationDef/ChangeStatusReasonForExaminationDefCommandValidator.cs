using FluentValidation;
namespace KLMN.Application.ReasonForExaminations.Commands.ChangeStatusReasonForExaminationDef;
/// <summary>ChangeStatus Muayene Sebebi alan ve concurrency doğrulaması.</summary>
public sealed class ChangeStatusReasonForExaminationDefCommandValidator:AbstractValidator<ChangeStatusReasonForExaminationDefCommand>
{
    public ChangeStatusReasonForExaminationDefCommandValidator()
    {
      
      RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
    }
}
