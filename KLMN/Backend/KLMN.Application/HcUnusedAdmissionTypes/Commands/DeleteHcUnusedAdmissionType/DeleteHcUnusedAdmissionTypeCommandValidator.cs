using FluentValidation;
namespace KLMN.Application.HcUnusedAdmissionTypes.Commands.DeleteHcUnusedAdmissionType;
/// <summary>Kimlik ve zorunlu FK alanlarını doğrular.</summary>
public sealed class DeleteHcUnusedAdmissionTypeCommandValidator:AbstractValidator<DeleteHcUnusedAdmissionTypeCommand>
{
 public DeleteHcUnusedAdmissionTypeCommandValidator(){RuleFor(x=>x.Id).NotEmpty();}
}
