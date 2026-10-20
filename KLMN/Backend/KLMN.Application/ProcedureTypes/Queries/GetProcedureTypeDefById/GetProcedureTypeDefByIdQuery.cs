using KLMN.Application.ProcedureTypes.Responses;
using MediatR;
namespace KLMN.Application.ProcedureTypes.Queries.GetProcedureTypeDefById;
/// <summary>Tetkik türünü kimlikle getirir.</summary>
public sealed record GetProcedureTypeDefByIdQuery(Guid Id):IRequest<ProcedureTypeDefResponse>;
