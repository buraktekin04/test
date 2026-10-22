using FluentValidation;
 using KLMN.Domain.Common;
 namespace KLMN.Application.PlaneTypes.Commands.CreatePlaneTypeDef;
 /// <summary>Uçuşa Uçak Tipi Tanımları Create doğrulaması.</summary>
 public sealed class CreatePlaneTypeDefCommandValidator:AbstractValidator<CreatePlaneTypeDefCommand>
 {
 public CreatePlaneTypeDefCommandValidator(){
 
 
 RuleFor(x=>x.Name).NotEmpty().MaximumLength(250);
 
 RuleFor(x=>x.Code).NotEmpty().MaximumLength(100);
 
 RuleFor(x=>x).Must(x=>ValidityPeriod.HasValidBounds(x.ValidFrom,x.ValidTo)).WithMessage("Geçerlilik bitişi başlangıçtan sonra olmalıdır.");
 RuleFor(x=>x.ValidFrom).Must(x=>!x.HasValue||x.Value.Kind==DateTimeKind.Utc);
 RuleFor(x=>x.ValidTo).Must(x=>!x.HasValue||x.Value.Kind==DateTimeKind.Utc);
 }
 }
