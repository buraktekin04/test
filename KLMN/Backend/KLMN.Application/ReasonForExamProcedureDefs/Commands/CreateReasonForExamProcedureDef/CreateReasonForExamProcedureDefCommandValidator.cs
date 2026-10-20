using FluentValidation;
namespace KLMN.Application.ReasonForExamProcedureDefs.Commands.CreateReasonForExamProcedureDef;
/// <summary>Gereken Tetkikler isteği için alan ve sürüm kontrolü.</summary>
public sealed class CreateReasonForExamProcedureDefCommandValidator:AbstractValidator<CreateReasonForExamProcedureDefCommand>
{
 public CreateReasonForExamProcedureDefCommandValidator(){
 RuleFor(x=>x.ReasonForExaminationDefId).NotEmpty();RuleFor(x=>x.ReferenceId).NotEmpty();
 RuleFor(x=>x.StartingAge).InclusiveBetween(0,130);
 RuleFor(x=>x.EndingAge).InclusiveBetween(0,130);
 RuleFor(x=>x).Must(x=>x.StartingAge<=x.EndingAge).WithMessage("Bitiş yaşı başlangıç yaşından küçük olamaz.");
 RuleFor(x=>x.Gender).InclusiveBetween(0,20);
 
 }
}
