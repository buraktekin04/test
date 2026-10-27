using MediatR;
using KLMN.Application.EquivalentProcedures.Responses;
namespace KLMN.Application.EquivalentProcedures.Commands.ChangeStatusEquivalentProcedure;
/// <summary>ChangeStatus eşdeğer tetkik CQRS komutu.</summary>
public sealed record ChangeStatusEquivalentProcedureCommand(Guid Id,uint Version,bool IsActive):IRequest<EquivalentProcedureMutationResponse>;
