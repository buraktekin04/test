using FluentValidation;
namespace KLMN.Application.EquivalentProcedures.Commands.UpdateEquivalentProcedure;
/// <summary>Boş, öz-eşleştirme ve kayıt kimliklerinin denetimi.</summary>
public sealed class UpdateEquivalentProcedureCommandValidator:AbstractValidator<UpdateEquivalentProcedureCommand>
{
 public UpdateEquivalentProcedureCommandValidator(){
  RuleFor(x=>x.Id).NotEmpty();
  RuleFor(x=>x.ProcedureDefId).NotEmpty();
  RuleFor(x=>x.EquivalentProcedureId).NotEmpty();
  RuleFor(x=>x).Must(x=>x.ProcedureDefId!=x.EquivalentProcedureId)
    .WithMessage("Tetkik kendisiyle eşleştirilemez.");
 }
}
