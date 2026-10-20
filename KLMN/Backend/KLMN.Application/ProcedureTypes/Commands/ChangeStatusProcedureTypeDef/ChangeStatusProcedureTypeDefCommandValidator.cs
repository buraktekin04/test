using FluentValidation;
namespace KLMN.Application.ProcedureTypes.Commands.ChangeStatusProcedureTypeDef;
/// <summary>Tetkik türünün kod, kimlik ve isim kontrolü.</summary>
public sealed class ChangeStatusProcedureTypeDefCommandValidator:AbstractValidator<ChangeStatusProcedureTypeDefCommand>
{
    public ChangeStatusProcedureTypeDefCommandValidator()
    {
        
        
        RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
    }
}
