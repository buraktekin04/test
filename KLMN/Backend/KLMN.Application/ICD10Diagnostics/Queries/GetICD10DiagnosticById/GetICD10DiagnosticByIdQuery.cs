using KLMN.Application.ICD10Diagnostics.Responses;
using MediatR;
namespace KLMN.Application.ICD10Diagnostics.Queries.GetICD10DiagnosticById;
public sealed record GetICD10DiagnosticByIdQuery(Guid Id):IRequest<ICD10DiagnosticResponse>;
