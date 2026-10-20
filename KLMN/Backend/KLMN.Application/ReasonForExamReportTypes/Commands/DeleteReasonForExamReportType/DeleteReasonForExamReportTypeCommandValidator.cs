using FluentValidation;
namespace KLMN.Application.ReasonForExamReportTypes.Commands.DeleteReasonForExamReportType;
/// <summary>Muayene Sebebi Rapor Türleri isteği için alan ve sürüm kontrolü.</summary>
public sealed class DeleteReasonForExamReportTypeCommandValidator:AbstractValidator<DeleteReasonForExamReportTypeCommand>
{
 public DeleteReasonForExamReportTypeCommandValidator(){
 
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 }
}
