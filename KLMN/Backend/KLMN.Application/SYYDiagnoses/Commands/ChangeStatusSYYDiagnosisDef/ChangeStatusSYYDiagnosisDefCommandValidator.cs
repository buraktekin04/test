using FluentValidation;
 using KLMN.Domain.Common;
 namespace KLMN.Application.SYYDiagnoses.Commands.ChangeStatusSYYDiagnosisDef;
 /// <summary>SYY Tanı Listesi (M/D/F) ChangeStatus doğrulaması.</summary>
 public sealed class ChangeStatusSYYDiagnosisDefCommandValidator:AbstractValidator<ChangeStatusSYYDiagnosisDefCommand>
 {
 public ChangeStatusSYYDiagnosisDefCommandValidator(){
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
 }
