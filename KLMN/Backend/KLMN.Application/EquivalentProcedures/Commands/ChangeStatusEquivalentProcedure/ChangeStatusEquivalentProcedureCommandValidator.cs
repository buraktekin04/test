using FluentValidation;
namespace KLMN.Application.EquivalentProcedures.Commands.ChangeStatusEquivalentProcedure;
/// <summary>Boş, öz-eşleştirme ve kayıt kimliklerinin denetimi.</summary>
public sealed class ChangeStatusEquivalentProcedureCommandValidator:AbstractValidator<ChangeStatusEquivalentProcedureCommand>
{
 public ChangeStatusEquivalentProcedureCommandValidator(){
  RuleFor(x=>x.Id).NotEmpty();
  
 }
}
