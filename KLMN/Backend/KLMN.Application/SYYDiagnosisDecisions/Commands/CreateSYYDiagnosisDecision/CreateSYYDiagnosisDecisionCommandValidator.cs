using FluentValidation;
namespace KLMN.Application.SYYDiagnosisDecisions.Commands.CreateSYYDiagnosisDecision;
/// <summary>İstek alanlarını ve negatif olmayan miktarı denetler.</summary>
public sealed class CreateSYYDiagnosisDecisionCommandValidator:AbstractValidator<CreateSYYDiagnosisDecisionCommand>
{
 public CreateSYYDiagnosisDecisionCommandValidator(){RuleFor(x=>x.SYYDiagnosisDefId).NotEmpty();
   RuleFor(x=>x.HcDecisionDefId).NotEmpty();
   RuleFor(x=>x.HcAdmissionTypeDefId).NotEmpty();
   RuleFor(x=>x.Amount).GreaterThanOrEqualTo(0);}
}
