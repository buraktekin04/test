using FluentValidation;
namespace KLMN.Application.HcUnusedAdmissionTypes.Commands.UpdateHcUnusedAdmissionType;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class UpdateHcUnusedAdmissionTypeCommandValidator:AbstractValidator<UpdateHcUnusedAdmissionTypeCommand>
{
 public UpdateHcUnusedAdmissionTypeCommandValidator(){RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.HcDecisionDefId).NotEmpty();RuleFor(x=>x.ReferenceId).NotEmpty();}
}
