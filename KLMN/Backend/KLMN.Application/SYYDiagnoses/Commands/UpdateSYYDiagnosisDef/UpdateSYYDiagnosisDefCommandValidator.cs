using FluentValidation;
 using KLMN.Domain.Common;
 namespace KLMN.Application.SYYDiagnoses.Commands.UpdateSYYDiagnosisDef;
 /// <summary>SYY Tanı Listesi (M/D/F) Update doğrulaması.</summary>
 public sealed class UpdateSYYDiagnosisDefCommandValidator:AbstractValidator<UpdateSYYDiagnosisDefCommand>
 {
 public UpdateSYYDiagnosisDefCommandValidator(){
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 
 RuleFor(x=>x.Name).NotEmpty().MaximumLength(500);
 
 RuleFor(x=>x.Code).NotEmpty().MaximumLength(100);
 RuleFor(x=>x.SYYDiagnosisType).GreaterThanOrEqualTo(0);
 RuleFor(x=>x).Must(x=>ValidityPeriod.HasValidBounds(x.ValidFrom,x.ValidTo)).WithMessage("Geçerlilik bitişi başlangıçtan sonra olmalıdır.");
 RuleFor(x=>x.ValidFrom).Must(x=>!x.HasValue||x.Value.Kind==DateTimeKind.Utc);
 RuleFor(x=>x.ValidTo).Must(x=>!x.HasValue||x.Value.Kind==DateTimeKind.Utc);
 }
 }
