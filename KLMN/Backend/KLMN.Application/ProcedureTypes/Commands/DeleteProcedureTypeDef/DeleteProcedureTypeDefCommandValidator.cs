using FluentValidation;
namespace KLMN.Application.ProcedureTypes.Commands.DeleteProcedureTypeDef;
/// <summary>Tetkik türünün kod, kimlik ve isim kontrolü.</summary>
public sealed class DeleteProcedureTypeDefCommandValidator:AbstractValidator<DeleteProcedureTypeDefCommand>
{
    public DeleteProcedureTypeDefCommandValidator()
    {
        
        
        RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
    }
}
