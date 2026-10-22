using FluentValidation;
 
 namespace KLMN.Application.SYYDiagnosisSlices.Commands.UpdateSYYDiagnosisSliceDef;
 /// <summary>SYY Dilim Tanımları Update doğrulaması.</summary>
 public sealed class UpdateSYYDiagnosisSliceDefCommandValidator:AbstractValidator<UpdateSYYDiagnosisSliceDefCommand>
 {
 public UpdateSYYDiagnosisSliceDefCommandValidator(){
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 RuleFor(x=>x.Name).NotEmpty().MaximumLength(150);
 
 
 
 }
 }
