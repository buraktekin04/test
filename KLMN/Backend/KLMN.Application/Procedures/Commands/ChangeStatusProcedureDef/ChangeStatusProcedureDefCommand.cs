using KLMN.Application.Procedures.Responses;
using MediatR;

namespace KLMN.Application.Procedures.Commands.ChangeStatusProcedureDef;

/// <summary>Tetkik ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusProcedureDefCommand(Guid Id, uint Version, bool IsActive) : IRequest<ProcedureDefMutationResponse>;
