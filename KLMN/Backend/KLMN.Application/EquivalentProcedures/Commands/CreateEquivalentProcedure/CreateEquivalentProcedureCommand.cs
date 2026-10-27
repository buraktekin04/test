using MediatR;
using KLMN.Application.EquivalentProcedures.Responses;
namespace KLMN.Application.EquivalentProcedures.Commands.CreateEquivalentProcedure;
/// <summary>Create eşdeğer tetkik CQRS komutu.</summary>
public sealed record CreateEquivalentProcedureCommand(Guid ProcedureDefId,Guid EquivalentProcedureId):IRequest<EquivalentProcedureMutationResponse>;
