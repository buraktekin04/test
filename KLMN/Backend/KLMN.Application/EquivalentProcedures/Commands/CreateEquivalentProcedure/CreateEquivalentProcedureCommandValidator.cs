using FluentValidation;
namespace KLMN.Application.EquivalentProcedures.Commands.CreateEquivalentProcedure;
/// <summary>Boş, öz-eşleştirme ve kayıt kimliklerinin denetimi.</summary>
public sealed class CreateEquivalentProcedureCommandValidator:AbstractValidator<CreateEquivalentProcedureCommand>
{
 public CreateEquivalentProcedureCommandValidator(){
  
  RuleFor(x=>x.ProcedureDefId).NotEmpty();
  RuleFor(x=>x.EquivalentProcedureId).NotEmpty();
  RuleFor(x=>x).Must(x=>x.ProcedureDefId!=x.EquivalentProcedureId)
    .WithMessage("Tetkik kendisiyle eşleştirilemez.");
 }
}
