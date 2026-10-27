using MediatR;

namespace KLMN.Application.EquivalentProcedures.Commands.DeleteEquivalentProcedure;
/// <summary>Delete eşdeğer tetkik CQRS komutu.</summary>
public sealed record DeleteEquivalentProcedureCommand(Guid Id,uint Version):IRequest;
