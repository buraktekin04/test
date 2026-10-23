using FluentValidation;
namespace KLMN.Application.ICD10Diagnostics.Commands.ChangeStatusICD10Diagnostic;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları: ChangeStatus alan doğrulamaları.</summary>
public sealed class ChangeStatusICD10DiagnosticCommandValidator:AbstractValidator<ChangeStatusICD10DiagnosticCommand>
{
 public ChangeStatusICD10DiagnosticCommandValidator()
 {
 RuleFor(x=>x.Id).NotEmpty(); RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
}
