using FluentValidation;
namespace KLMN.Application.ProcedureTypes.Commands.CreateProcedureTypeDef;
/// <summary>Tetkik türünün kod, kimlik ve isim kontrolü.</summary>
public sealed class CreateProcedureTypeDefCommandValidator:AbstractValidator<CreateProcedureTypeDefCommand>
{
    public CreateProcedureTypeDefCommandValidator()
    {
        RuleFor(x=>x.Code).GreaterThanOrEqualTo(0);
        RuleFor(x=>x.Name).NotEmpty().MaximumLength(200);
        
    }
}
