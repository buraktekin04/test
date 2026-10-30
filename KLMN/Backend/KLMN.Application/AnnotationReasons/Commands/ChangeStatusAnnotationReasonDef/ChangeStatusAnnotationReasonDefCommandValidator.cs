using FluentValidation;
 using KLMN.Domain.Common;
 namespace KLMN.Application.AnnotationReasons.Commands.ChangeStatusAnnotationReasonDef;
 /// <summary>Zeyil Sebebi Tanımları ChangeStatus doğrulaması.</summary>
 public sealed class ChangeStatusAnnotationReasonDefCommandValidator:AbstractValidator<ChangeStatusAnnotationReasonDefCommand>
 {
 public ChangeStatusAnnotationReasonDefCommandValidator(){
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
 }
