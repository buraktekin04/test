using FluentValidation;
 using KLMN.Domain.Common;
 namespace KLMN.Application.AnnotationReasons.Commands.DeleteAnnotationReasonDef;
 /// <summary>Zeyil Sebebi Tanımları Delete doğrulaması.</summary>
 public sealed class DeleteAnnotationReasonDefCommandValidator:AbstractValidator<DeleteAnnotationReasonDefCommand>
 {
 public DeleteAnnotationReasonDefCommandValidator(){
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
 }
