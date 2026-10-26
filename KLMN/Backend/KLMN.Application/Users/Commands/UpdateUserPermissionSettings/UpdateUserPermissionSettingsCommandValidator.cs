using FluentValidation;
namespace KLMN.Application.Users.Commands.UpdateUserPermissionSettings;
/// <summary>Tekrarlanan veya boş permission kimliklerini reddeder.</summary>
public sealed class UpdateUserPermissionSettingsCommandValidator:AbstractValidator<UpdateUserPermissionSettingsCommand>
{
 public UpdateUserPermissionSettingsCommandValidator()
 {
   RuleFor(x=>x.UserId).NotEmpty();
   RuleFor(x=>x.Choices).NotNull().Must(x=>x is not null && x.Count<=2000 &&
      x.All(p=>p.PermissionId!=Guid.Empty) &&
      x.Select(p=>p.PermissionId).Distinct().Count()==x.Count)
      .WithMessage("Geçersiz veya tekrarlanan özel yetki seçimi.");
 }
}
