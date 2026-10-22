using MediatR;
namespace KLMN.Application.SYYDiagnosisBranches.Queries.GetSYYDiagnosisBranchDefOptions;
/// <summary>Kendi sayfasının yetkisi ile referans seçeneklerini döndürür.</summary>
public sealed record SYYReferenceOption(Guid Id,string Name,bool IsActive);
public sealed record SYYDiagnosisBranchDefOptions(IReadOnlyList<SYYReferenceOption> Diagnoses,
 IReadOnlyList<SYYReferenceOption> References);
public sealed record GetSYYDiagnosisBranchDefOptionsQuery(Guid? SelectedSYYDiagnosisDefId=null,Guid? SelectedBranchDefId=null):IRequest<SYYDiagnosisBranchDefOptions>;
