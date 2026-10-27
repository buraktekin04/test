using KLMN.Application.EquivalentProcedures.Responses;
using MediatR;
namespace KLMN.Application.EquivalentProcedures.Queries.GetEquivalentProcedureById;
public sealed record GetEquivalentProcedureByIdQuery(Guid Id):IRequest<EquivalentProcedureResponse>;
