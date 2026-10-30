using FluentValidation;

namespace KLMN.Application.Dispatches.Commands.DeleteDispatchPhoto;

/// <summary>Fotoğraf silme isteğinin optimistic concurrency kontrol alanları.</summary>
public sealed class DeleteDispatchPhotoCommandValidator:AbstractValidator<DeleteDispatchPhotoCommand>
{
    public DeleteDispatchPhotoCommandValidator()
    {
        RuleFor(x=>x.Id).NotEmpty();
        RuleFor(x=>x.Version).GreaterThan(0U);
    }
}
