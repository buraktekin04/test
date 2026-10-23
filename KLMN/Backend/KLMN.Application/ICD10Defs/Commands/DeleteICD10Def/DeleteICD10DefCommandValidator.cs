using FluentValidation;
namespace KLMN.Application.ICD10Defs.Commands.DeleteICD10Def;
/// <summary>ICD10 Tanımları: Delete alan doğrulamaları.</summary>
public sealed class DeleteICD10DefCommandValidator:AbstractValidator<DeleteICD10DefCommand>
{
 public DeleteICD10DefCommandValidator()
 {
 RuleFor(x=>x.Id).NotEmpty(); RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
}
