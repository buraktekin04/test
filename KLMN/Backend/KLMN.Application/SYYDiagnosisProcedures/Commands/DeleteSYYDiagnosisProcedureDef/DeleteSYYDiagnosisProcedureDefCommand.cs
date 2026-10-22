using MediatR;

namespace KLMN.Application.SYYDiagnosisProcedures.Commands.DeleteSYYDiagnosisProcedureDef;
/// <summary>SYY Gereken Tetkikler Delete CQRS komutu.</summary>
public sealed record DeleteSYYDiagnosisProcedureDefCommand(Guid Id,uint Version):IRequest;
