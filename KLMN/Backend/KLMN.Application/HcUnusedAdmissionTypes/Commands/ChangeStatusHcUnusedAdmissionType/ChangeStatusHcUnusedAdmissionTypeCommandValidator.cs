using FluentValidation;
namespace KLMN.Application.HcUnusedAdmissionTypes.Commands.ChangeStatusHcUnusedAdmissionType;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class ChangeStatusHcUnusedAdmissionTypeCommandValidator:AbstractValidator<ChangeStatusHcUnusedAdmissionTypeCommand>
{
 public ChangeStatusHcUnusedAdmissionTypeCommandValidator(){RuleFor(x=>x.Id).NotEmpty();}
}
