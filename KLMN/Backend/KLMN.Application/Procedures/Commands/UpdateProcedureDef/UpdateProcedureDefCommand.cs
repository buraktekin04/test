using KLMN.Application.Procedures.Responses;
using MediatR;

namespace KLMN.Application.Procedures.Commands.UpdateProcedureDef;

/// <summary>Tetkik Update CQRS komutu.</summary>
public sealed record UpdateProcedureDefCommand(Guid Id, uint Version, string ProcedureCode, string Name, bool RequiresNumericResult, bool RequiresReferenceRange, bool RequiresDeviationInfo, DateTime? StartDate, DateTime? EndDate, int ProcedureType) : IRequest<ProcedureDefMutationResponse>;
