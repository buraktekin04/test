using MediatR;
using KLMN.Application.ICD10Diagnostics.Responses;
namespace KLMN.Application.ICD10Diagnostics.Commands.UpdateICD10Diagnostic;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları: Update CQRS komutu.</summary>
public sealed record UpdateICD10DiagnosticCommand(Guid Id,uint Version,Guid ICD10DefId,Guid SYYDiagnosisDefId,Guid HcAdmissionTypeDefId,int Amount):IRequest<ICD10DiagnosticMutationResponse>;
