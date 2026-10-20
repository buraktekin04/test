using FluentValidation;
namespace KLMN.Application.ReasonForExamReportTypes.Commands.CreateReasonForExamReportType;
/// <summary>Muayene Sebebi Rapor Türleri isteği için alan ve sürüm kontrolü.</summary>
public sealed class CreateReasonForExamReportTypeCommandValidator:AbstractValidator<CreateReasonForExamReportTypeCommand>
{
 public CreateReasonForExamReportTypeCommandValidator(){
 RuleFor(x=>x.ReasonForExaminationDefId).NotEmpty();RuleFor(x=>x.ReferenceId).NotEmpty();
 
 
 }
}
