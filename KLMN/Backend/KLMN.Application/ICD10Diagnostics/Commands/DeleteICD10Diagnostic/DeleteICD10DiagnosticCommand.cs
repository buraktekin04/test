using MediatR;

namespace KLMN.Application.ICD10Diagnostics.Commands.DeleteICD10Diagnostic;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları: Delete CQRS komutu.</summary>
public sealed record DeleteICD10DiagnosticCommand(Guid Id,uint Version):IRequest;
