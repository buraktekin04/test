using KLMN.Application.ICD10Defs.Responses;
using MediatR;
namespace KLMN.Application.ICD10Defs.Queries.GetICD10DefById;
public sealed record GetICD10DefByIdQuery(Guid Id):IRequest<ICD10DefResponse>;
