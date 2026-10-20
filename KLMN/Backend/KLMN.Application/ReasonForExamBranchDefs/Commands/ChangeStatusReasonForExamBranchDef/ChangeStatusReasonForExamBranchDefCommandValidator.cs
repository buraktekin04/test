using FluentValidation;
namespace KLMN.Application.ReasonForExamBranchDefs.Commands.ChangeStatusReasonForExamBranchDef;
/// <summary>Muayene Gereken Branşlar isteği için alan ve sürüm kontrolü.</summary>
public sealed class ChangeStatusReasonForExamBranchDefCommandValidator:AbstractValidator<ChangeStatusReasonForExamBranchDefCommand>
{
 public ChangeStatusReasonForExamBranchDefCommandValidator(){
 
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 }
}
