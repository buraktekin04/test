using FluentValidation;
namespace KLMN.Application.ReasonForExamReportTypes.Commands.ChangeStatusReasonForExamReportType;
/// <summary>Muayene Sebebi Rapor Türleri isteği için alan ve sürüm kontrolü.</summary>
public sealed class ChangeStatusReasonForExamReportTypeCommandValidator:AbstractValidator<ChangeStatusReasonForExamReportTypeCommand>
{
 public ChangeStatusReasonForExamReportTypeCommandValidator(){
 
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 }
}
