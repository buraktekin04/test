using FluentValidation;
namespace KLMN.Application.ICD10Diagnostics.Commands.CreateICD10Diagnostic;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları: Create alan doğrulamaları.</summary>
public sealed class CreateICD10DiagnosticCommandValidator:AbstractValidator<CreateICD10DiagnosticCommand>
{
 public CreateICD10DiagnosticCommandValidator()
 {
 
 RuleFor(x=>x.ICD10DefId).NotEmpty();
  RuleFor(x=>x.SYYDiagnosisDefId).NotEmpty();
  RuleFor(x=>x.HcAdmissionTypeDefId).NotEmpty();
  RuleFor(x=>x.Amount).GreaterThanOrEqualTo(0);
 }
}
