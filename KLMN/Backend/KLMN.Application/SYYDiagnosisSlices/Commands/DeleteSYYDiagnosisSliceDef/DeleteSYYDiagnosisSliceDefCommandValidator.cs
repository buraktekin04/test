using FluentValidation;
 
 namespace KLMN.Application.SYYDiagnosisSlices.Commands.DeleteSYYDiagnosisSliceDef;
 /// <summary>SYY Dilim Tanımları Delete doğrulaması.</summary>
 public sealed class DeleteSYYDiagnosisSliceDefCommandValidator:AbstractValidator<DeleteSYYDiagnosisSliceDefCommand>
 {
 public DeleteSYYDiagnosisSliceDefCommandValidator(){
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
 }
