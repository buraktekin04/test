using MediatR;
using KLMN.Application.SYYDiagnosisProcedures.Responses;
namespace KLMN.Application.SYYDiagnosisProcedures.Commands.ChangeStatusSYYDiagnosisProcedureDef;
/// <summary>SYY Gereken Tetkikler ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusSYYDiagnosisProcedureDefCommand(Guid Id,uint Version,bool IsActive):IRequest<SYYDiagnosisProcedureDefMutationResponse>;
