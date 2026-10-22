using FluentValidation;
 
 namespace KLMN.Application.SYYDiagnosisTypes.Commands.CreateSYYDiagnosisTypeDef;
 /// <summary>SYY Tanı Türü Tanımları Create doğrulaması.</summary>
 public sealed class CreateSYYDiagnosisTypeDefCommandValidator:AbstractValidator<CreateSYYDiagnosisTypeDefCommand>
 {
 public CreateSYYDiagnosisTypeDefCommandValidator(){
 
 
 RuleFor(x=>x.Name).NotEmpty().MaximumLength(150);
 RuleFor(x=>x.Code).GreaterThanOrEqualTo(0);
 
 
 }
 }
