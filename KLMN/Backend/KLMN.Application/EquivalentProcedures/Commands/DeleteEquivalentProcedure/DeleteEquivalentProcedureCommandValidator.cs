using FluentValidation;
namespace KLMN.Application.EquivalentProcedures.Commands.DeleteEquivalentProcedure;
/// <summary>Boş, öz-eşleştirme ve kayıt kimliklerinin denetimi.</summary>
public sealed class DeleteEquivalentProcedureCommandValidator:AbstractValidator<DeleteEquivalentProcedureCommand>
{
 public DeleteEquivalentProcedureCommandValidator(){
  RuleFor(x=>x.Id).NotEmpty();
  
 }
}
