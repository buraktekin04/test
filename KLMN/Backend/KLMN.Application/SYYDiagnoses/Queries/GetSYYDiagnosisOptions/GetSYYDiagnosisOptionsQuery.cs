using MediatR;
namespace KLMN.Application.SYYDiagnoses.Queries.GetSYYDiagnosisOptions;
/// <summary>Ana form için üç referans listesi: Tanı Türü, Dilim, Bağlı Kayıt.</summary>
public sealed record SYYOption<T>(T Value,string Label,bool IsActive);
public sealed record SYYDiagnosisOptions(
 IReadOnlyList<SYYOption<int>> Types,IReadOnlyList<SYYOption<int>> Slices,
 IReadOnlyList<SYYOption<Guid>> Parents);
public sealed record GetSYYDiagnosisOptionsQuery(Guid? CurrentId=null,int? CurrentType=null,
 int? CurrentSlice=null,Guid? CurrentParent=null):IRequest<SYYDiagnosisOptions>;
