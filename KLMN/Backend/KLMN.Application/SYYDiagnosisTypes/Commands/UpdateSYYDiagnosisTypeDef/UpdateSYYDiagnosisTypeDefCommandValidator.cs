using FluentValidation;
 
 namespace KLMN.Application.SYYDiagnosisTypes.Commands.UpdateSYYDiagnosisTypeDef;
 /// <summary>SYY Tanı Türü Tanımları Update doğrulaması.</summary>
 public sealed class UpdateSYYDiagnosisTypeDefCommandValidator:AbstractValidator<UpdateSYYDiagnosisTypeDefCommand>
 {
 public UpdateSYYDiagnosisTypeDefCommandValidator(){
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 RuleFor(x=>x.Name).NotEmpty().MaximumLength(150);
 
 
 
 }
 }
