using MediatR;
namespace KLMN.Application.SYYDiagnosisRequireds.Queries.GetSYYDiagnosisRequiredOptions;
public sealed record RequiredDiagnosisOption(Guid Id,string Name,bool IsActive);
public sealed record GetSYYDiagnosisRequiredOptionsQuery(Guid? SelectedDiagnosisId=null):
 IRequest<IReadOnlyList<RequiredDiagnosisOption>>;
