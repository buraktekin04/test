using MediatR;
namespace KLMN.Application.SYYClassifications.Queries.GetSYYClassificationDefOptions;
/// <summary>Kendi sayfasının yetkisi ile referans seçeneklerini döndürür.</summary>
public sealed record SYYReferenceOption(Guid Id,string Name,bool IsActive);
public sealed record SYYClassificationDefOptions(IReadOnlyList<SYYReferenceOption> Diagnoses,
 IReadOnlyList<SYYReferenceOption> Forces,IReadOnlyList<SYYReferenceOption> Ranks,
 IReadOnlyList<SYYReferenceOption> Classes,IReadOnlyList<SYYReferenceOption> Decisions);
public sealed record GetSYYClassificationDefOptionsQuery(Guid? SelectedSYYDiagnosisDefId=null,Guid? SelectedForcesCommandDef=null,Guid? SelectedRankGroup=null,Guid? SelectedMilitaryClassGroup=null,Guid? SelectedHcDecision=null):IRequest<SYYClassificationDefOptions>;
