using FluentValidation;
 
 namespace KLMN.Application.SYYDiagnosisSlices.Commands.ChangeStatusSYYDiagnosisSliceDef;
 /// <summary>SYY Dilim Tanımları ChangeStatus doğrulaması.</summary>
 public sealed class ChangeStatusSYYDiagnosisSliceDefCommandValidator:AbstractValidator<ChangeStatusSYYDiagnosisSliceDefCommand>
 {
 public ChangeStatusSYYDiagnosisSliceDefCommandValidator(){
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
 }
