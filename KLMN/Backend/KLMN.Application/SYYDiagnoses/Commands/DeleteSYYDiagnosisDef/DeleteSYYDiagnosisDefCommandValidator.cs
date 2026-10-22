using FluentValidation;
 using KLMN.Domain.Common;
 namespace KLMN.Application.SYYDiagnoses.Commands.DeleteSYYDiagnosisDef;
 /// <summary>SYY Tanı Listesi (M/D/F) Delete doğrulaması.</summary>
 public sealed class DeleteSYYDiagnosisDefCommandValidator:AbstractValidator<DeleteSYYDiagnosisDefCommand>
 {
 public DeleteSYYDiagnosisDefCommandValidator(){
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 }
 }
