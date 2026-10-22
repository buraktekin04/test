using FluentValidation;
 
 namespace KLMN.Application.SYYDiagnosisTypes.Commands.ChangeStatusSYYDiagnosisTypeDef;
 /// <summary>SYY Tanı Türü Tanımları ChangeStatus doğrulaması.</summary>
 public sealed class ChangeStatusSYYDiagnosisTypeDefCommandValidator:AbstractValidator<ChangeStatusSYYDiagnosisTypeDefCommand>
 {
 public ChangeStatusSYYDiagnosisTypeDefCommandValidator(){
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
 }
