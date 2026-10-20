using FluentValidation;
namespace KLMN.Application.ReasonForExamProcedureDefs.Commands.ChangeStatusReasonForExamProcedureDef;
/// <summary>Gereken Tetkikler isteği için alan ve sürüm kontrolü.</summary>
public sealed class ChangeStatusReasonForExamProcedureDefCommandValidator:AbstractValidator<ChangeStatusReasonForExamProcedureDefCommand>
{
 public ChangeStatusReasonForExamProcedureDefCommandValidator(){
 
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 }
}
