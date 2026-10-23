using FluentValidation;
namespace KLMN.Application.ICD10Diagnostics.Commands.UpdateICD10Diagnostic;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları: Update alan doğrulamaları.</summary>
public sealed class UpdateICD10DiagnosticCommandValidator:AbstractValidator<UpdateICD10DiagnosticCommand>
{
 public UpdateICD10DiagnosticCommandValidator()
 {
 RuleFor(x=>x.Id).NotEmpty(); RuleFor(x=>x.Version).GreaterThan(0u);
 RuleFor(x=>x.ICD10DefId).NotEmpty();
  RuleFor(x=>x.SYYDiagnosisDefId).NotEmpty();
  RuleFor(x=>x.HcAdmissionTypeDefId).NotEmpty();
  RuleFor(x=>x.Amount).GreaterThanOrEqualTo(0);
 }
}
