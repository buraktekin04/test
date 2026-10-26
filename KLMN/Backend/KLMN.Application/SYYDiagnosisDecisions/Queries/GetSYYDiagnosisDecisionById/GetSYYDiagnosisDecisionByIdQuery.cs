using KLMN.Application.SYYDiagnosisDecisions.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnosisDecisions.Queries.GetSYYDiagnosisDecisionById;
/// <summary>Tek istatistik satırını getirir.</summary>
public sealed record GetSYYDiagnosisDecisionByIdQuery(Guid Id):IRequest<SYYDiagnosisDecisionResponse>;
