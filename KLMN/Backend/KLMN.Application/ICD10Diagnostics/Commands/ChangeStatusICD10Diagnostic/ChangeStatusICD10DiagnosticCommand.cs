using MediatR;
using KLMN.Application.ICD10Diagnostics.Responses;
namespace KLMN.Application.ICD10Diagnostics.Commands.ChangeStatusICD10Diagnostic;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları: ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusICD10DiagnosticCommand(Guid Id,uint Version,bool IsActive):IRequest<ICD10DiagnosticMutationResponse>;
