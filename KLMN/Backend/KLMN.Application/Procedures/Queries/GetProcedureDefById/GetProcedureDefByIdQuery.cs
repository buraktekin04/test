using KLMN.Application.Procedures.Responses;
using MediatR;

namespace KLMN.Application.Procedures.Queries.GetProcedureDefById;

/// <summary>xmin içeren tetkik detay sorgusu.</summary>
public sealed record GetProcedureDefByIdQuery(Guid Id) : IRequest<ProcedureDefResponse>;
