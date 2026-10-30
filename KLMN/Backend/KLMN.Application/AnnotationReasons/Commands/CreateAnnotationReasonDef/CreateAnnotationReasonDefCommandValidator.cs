using FluentValidation;
 using KLMN.Domain.Common;
 namespace KLMN.Application.AnnotationReasons.Commands.CreateAnnotationReasonDef;
 /// <summary>Zeyil Sebebi Tanımları Create doğrulaması.</summary>
 public sealed class CreateAnnotationReasonDefCommandValidator:AbstractValidator<CreateAnnotationReasonDefCommand>
 {
 public CreateAnnotationReasonDefCommandValidator(){
 
 
 RuleFor(x=>x.Name).NotEmpty().MaximumLength(250);
 
 RuleFor(x=>x.Code).NotEmpty().MaximumLength(100);
 
 RuleFor(x=>x).Must(x=>ValidityPeriod.HasValidBounds(x.ValidFrom,x.ValidTo)).WithMessage("Geçerlilik bitişi başlangıçtan sonra olmalıdır.");
 RuleFor(x=>x.ValidFrom).Must(x=>!x.HasValue||x.Value.Kind==DateTimeKind.Utc);
 RuleFor(x=>x.ValidTo).Must(x=>!x.HasValue||x.Value.Kind==DateTimeKind.Utc);
 }
 }
