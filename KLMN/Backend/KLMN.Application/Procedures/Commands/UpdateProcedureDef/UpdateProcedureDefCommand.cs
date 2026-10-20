using KLMN.Application.Procedures.Responses;
using MediatR;

namespace KLMN.Application.Procedures.Commands.UpdateProcedureDef;

/// <summary>Tetkik Update CQRS komutu.</summary>
public sealed record UpdateProcedureDefCommand(Guid Id, uint Version, string Code, string Name, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<ProcedureDefMutationResponse>;
