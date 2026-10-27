using MediatR;
using KLMN.Application.EquivalentProcedures.Responses;
namespace KLMN.Application.EquivalentProcedures.Commands.UpdateEquivalentProcedure;
/// <summary>Update eşdeğer tetkik CQRS komutu.</summary>
public sealed record UpdateEquivalentProcedureCommand(Guid Id,uint Version,Guid ProcedureDefId,Guid EquivalentProcedureId):IRequest<EquivalentProcedureMutationResponse>;
