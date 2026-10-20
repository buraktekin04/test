using FluentValidation;
namespace KLMN.Application.ProcedureTypes.Commands.UpdateProcedureTypeDef;
/// <summary>Tetkik türünün kod, kimlik ve isim kontrolü.</summary>
public sealed class UpdateProcedureTypeDefCommandValidator:AbstractValidator<UpdateProcedureTypeDefCommand>
{
    public UpdateProcedureTypeDefCommandValidator()
    {
        
        RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
    }
}
