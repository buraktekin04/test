using KLMN.Application.Procedures.Responses;
using MediatR;

namespace KLMN.Application.Procedures.Commands.CreateProcedureDef;

/// <summary>Tetkik Create CQRS komutu.</summary>
public sealed record CreateProcedureDefCommand(string ProcedureCode, string Name, bool RequiresNumericResult, bool RequiresReferenceRange, bool RequiresDeviationInfo, DateTime? ValidFrom, DateTime? ValidTo, int ProcedureType) : IRequest<ProcedureDefMutationResponse>;
