using FluentValidation;
namespace KLMN.Application.ICD10Diagnostics.Commands.DeleteICD10Diagnostic;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları: Delete alan doğrulamaları.</summary>
public sealed class DeleteICD10DiagnosticCommandValidator:AbstractValidator<DeleteICD10DiagnosticCommand>
{
 public DeleteICD10DiagnosticCommandValidator()
 {
 RuleFor(x=>x.Id).NotEmpty(); RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
}
