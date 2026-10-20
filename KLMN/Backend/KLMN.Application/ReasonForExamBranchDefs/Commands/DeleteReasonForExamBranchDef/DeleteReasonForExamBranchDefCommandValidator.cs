using FluentValidation;
namespace KLMN.Application.ReasonForExamBranchDefs.Commands.DeleteReasonForExamBranchDef;
/// <summary>Muayene Gereken Branşlar isteği için alan ve sürüm kontrolü.</summary>
public sealed class DeleteReasonForExamBranchDefCommandValidator:AbstractValidator<DeleteReasonForExamBranchDefCommand>
{
 public DeleteReasonForExamBranchDefCommandValidator(){
 
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 }
}
