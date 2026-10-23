using MediatR;
using KLMN.Application.ICD10Diagnostics.Responses;
namespace KLMN.Application.ICD10Diagnostics.Commands.CreateICD10Diagnostic;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları: Create CQRS komutu.</summary>
public sealed record CreateICD10DiagnosticCommand(Guid ICD10DefId,Guid SYYDiagnosisDefId,Guid HcAdmissionTypeDefId,int Amount):IRequest<ICD10DiagnosticMutationResponse>;
