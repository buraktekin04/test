using FluentValidation;

namespace KLMN.Application.Dispatches.Commands.SetDispatchPhoto;

/// <summary>Fotoğraf için en çok 2 MiB boyut ve desteklenen MIME kontrolü.</summary>
public sealed class SetDispatchPhotoCommandValidator:AbstractValidator<SetDispatchPhotoCommand>
{
    public SetDispatchPhotoCommandValidator()
    {
        RuleFor(x=>x.Id).NotEmpty();
        RuleFor(x=>x.Content).NotNull().Must(bytes=>bytes is {Length: >0 and <=2097152})
            .WithMessage("Fotoğraf en fazla 2 MiB olabilir.");
        RuleFor(x=>x.ContentType).Must(x=>x=="image/jpeg"||x=="image/png")
            .WithMessage("Fotoğraf JPEG veya PNG olmalıdır.");
    }
}
