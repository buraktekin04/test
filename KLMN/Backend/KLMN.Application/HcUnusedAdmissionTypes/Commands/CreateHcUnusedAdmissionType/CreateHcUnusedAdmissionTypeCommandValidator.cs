using FluentValidation;
namespace KLMN.Application.HcUnusedAdmissionTypes.Commands.CreateHcUnusedAdmissionType;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class CreateHcUnusedAdmissionTypeCommandValidator:AbstractValidator<CreateHcUnusedAdmissionTypeCommand>
{
 public CreateHcUnusedAdmissionTypeCommandValidator(){RuleFor(x=>x.HcDecisionDefId).NotEmpty();RuleFor(x=>x.ReferenceId).NotEmpty();}
}
