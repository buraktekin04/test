using MediatR;

namespace KLMN.Application.Procedures.Commands.DeleteProcedureDef;

/// <summary>Tetkik Delete CQRS komutu.</summary>
public sealed record DeleteProcedureDefCommand(Guid Id, uint Version) : IRequest;
