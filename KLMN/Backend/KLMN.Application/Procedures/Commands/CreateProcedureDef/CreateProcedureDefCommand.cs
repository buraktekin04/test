using KLMN.Application.Procedures.Responses;
using MediatR;

namespace KLMN.Application.Procedures.Commands.CreateProcedureDef;

/// <summary>Tetkik Create CQRS komutu.</summary>
public sealed record CreateProcedureDefCommand(string Code, string Name, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<ProcedureDefMutationResponse>;
