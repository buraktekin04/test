using FluentValidation;
namespace KLMN.Application.SYYDiagnosisDecisions.Commands.DeleteSYYDiagnosisDecision;
/// <summary>İstek alanlarını ve negatif olmayan miktarı denetler.</summary>
public sealed class DeleteSYYDiagnosisDecisionCommandValidator:AbstractValidator<DeleteSYYDiagnosisDecisionCommand>
{
 public DeleteSYYDiagnosisDecisionCommandValidator(){RuleFor(x=>x.Id).NotEmpty();
   }
}
