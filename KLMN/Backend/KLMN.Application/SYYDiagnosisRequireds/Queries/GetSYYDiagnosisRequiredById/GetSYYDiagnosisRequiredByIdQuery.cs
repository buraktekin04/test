using KLMN.Application.SYYDiagnosisRequireds.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnosisRequireds.Queries.GetSYYDiagnosisRequiredById;
public sealed record GetSYYDiagnosisRequiredByIdQuery(Guid Id):IRequest<SYYDiagnosisRequiredResponse>;
