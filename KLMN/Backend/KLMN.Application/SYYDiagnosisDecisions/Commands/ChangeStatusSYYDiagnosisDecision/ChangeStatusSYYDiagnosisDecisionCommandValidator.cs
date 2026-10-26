using FluentValidation;
namespace KLMN.Application.SYYDiagnosisDecisions.Commands.ChangeStatusSYYDiagnosisDecision;
/// <summary>İstek alanlarını ve negatif olmayan miktarı denetler.</summary>
public sealed class ChangeStatusSYYDiagnosisDecisionCommandValidator:AbstractValidator<ChangeStatusSYYDiagnosisDecisionCommand>
{
 public ChangeStatusSYYDiagnosisDecisionCommandValidator(){RuleFor(x=>x.Id).NotEmpty();
   }
}
