using FluentValidation;
namespace KLMN.Application.ReasonForExamReportTypes.Commands.UpdateReasonForExamReportType;
/// <summary>Muayene Sebebi Rapor Türleri isteği için alan ve sürüm kontrolü.</summary>
public sealed class UpdateReasonForExamReportTypeCommandValidator:AbstractValidator<UpdateReasonForExamReportTypeCommand>
{
 public UpdateReasonForExamReportTypeCommandValidator(){
 RuleFor(x=>x.ReasonForExaminationDefId).NotEmpty();RuleFor(x=>x.ReferenceId).NotEmpty();
 
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 }
}
