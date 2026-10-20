using MediatR;
using KLMN.Application.ProcedureTypes.Responses;
namespace KLMN.Application.ProcedureTypes.Commands.ChangeStatusProcedureTypeDef;
/// <summary>Tetkik Türü Tanımları: ChangeStatus komutu.</summary>
public sealed record ChangeStatusProcedureTypeDefCommand(Guid Id,uint Version,bool IsActive):IRequest<ProcedureTypeDefMutationResponse>;
