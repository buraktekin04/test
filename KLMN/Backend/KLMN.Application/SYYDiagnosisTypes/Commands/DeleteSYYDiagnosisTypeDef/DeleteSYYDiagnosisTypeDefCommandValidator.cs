using FluentValidation;
 
 namespace KLMN.Application.SYYDiagnosisTypes.Commands.DeleteSYYDiagnosisTypeDef;
 /// <summary>SYY Tanı Türü Tanımları Delete doğrulaması.</summary>
 public sealed class DeleteSYYDiagnosisTypeDefCommandValidator:AbstractValidator<DeleteSYYDiagnosisTypeDefCommand>
 {
 public DeleteSYYDiagnosisTypeDefCommandValidator(){
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
 }
