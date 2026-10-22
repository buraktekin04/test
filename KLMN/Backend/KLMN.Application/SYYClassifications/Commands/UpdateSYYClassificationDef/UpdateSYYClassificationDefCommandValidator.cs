using FluentValidation;
using KLMN.Domain.Common;
namespace KLMN.Application.SYYClassifications.Commands.UpdateSYYClassificationDef;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi: Update doğrulama.</summary>
public sealed class UpdateSYYClassificationDefCommandValidator:AbstractValidator<UpdateSYYClassificationDefCommand>
{
 public UpdateSYYClassificationDefCommandValidator()
 {
 RuleFor(x=>x.Id).NotEmpty();RuleFor(x=>x.Version).GreaterThan(0u);
 RuleFor(x=>x.SYYDiagnosisDefId).NotEmpty();
RuleFor(x=>x.ForcesCommandDef).NotEmpty();
RuleFor(x=>x.RankGroup).NotEmpty();
RuleFor(x=>x.MilitaryClassGroup).NotEmpty();
RuleFor(x=>x.HcDecision).NotEmpty();
 RuleFor(x=>x).Must(x=>ValidityPeriod.HasValidBounds(x.ValidFrom,x.ValidTo));
 RuleFor(x=>x.ValidFrom).Must(x=>!x.HasValue||x.Value.Kind==DateTimeKind.Utc);
 RuleFor(x=>x.ValidTo).Must(x=>!x.HasValue||x.Value.Kind==DateTimeKind.Utc);
 }
}
