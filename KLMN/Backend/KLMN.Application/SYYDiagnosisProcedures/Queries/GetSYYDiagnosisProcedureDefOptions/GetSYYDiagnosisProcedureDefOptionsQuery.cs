using MediatR;
namespace KLMN.Application.SYYDiagnosisProcedures.Queries.GetSYYDiagnosisProcedureDefOptions;
/// <summary>Kendi sayfasının yetkisi ile referans seçeneklerini döndürür.</summary>
public sealed record SYYReferenceOption(Guid Id,string Name,bool IsActive);
public sealed record SYYDiagnosisProcedureDefOptions(IReadOnlyList<SYYReferenceOption> Diagnoses,
 IReadOnlyList<SYYReferenceOption> References);
public sealed record GetSYYDiagnosisProcedureDefOptionsQuery(Guid? SelectedSYYDiagnosisDefId=null,Guid? SelectedProcedureDefId=null):IRequest<SYYDiagnosisProcedureDefOptions>;
