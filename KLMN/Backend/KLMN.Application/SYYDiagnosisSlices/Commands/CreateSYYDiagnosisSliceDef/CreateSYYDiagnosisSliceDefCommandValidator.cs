using FluentValidation;
 
 namespace KLMN.Application.SYYDiagnosisSlices.Commands.CreateSYYDiagnosisSliceDef;
 /// <summary>SYY Dilim Tanımları Create doğrulaması.</summary>
 public sealed class CreateSYYDiagnosisSliceDefCommandValidator:AbstractValidator<CreateSYYDiagnosisSliceDefCommand>
 {
 public CreateSYYDiagnosisSliceDefCommandValidator(){
 
 
 RuleFor(x=>x.Name).NotEmpty().MaximumLength(150);
 RuleFor(x=>x.Code).GreaterThanOrEqualTo(0);
 
 
 }
 }
