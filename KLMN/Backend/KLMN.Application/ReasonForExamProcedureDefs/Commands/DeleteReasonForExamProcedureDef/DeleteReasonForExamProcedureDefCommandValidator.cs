using FluentValidation;
namespace KLMN.Application.ReasonForExamProcedureDefs.Commands.DeleteReasonForExamProcedureDef;
/// <summary>Gereken Tetkikler isteği için alan ve sürüm kontrolü.</summary>
public sealed class DeleteReasonForExamProcedureDefCommandValidator:AbstractValidator<DeleteReasonForExamProcedureDefCommand>
{
 public DeleteReasonForExamProcedureDefCommandValidator(){
 
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 }
}
