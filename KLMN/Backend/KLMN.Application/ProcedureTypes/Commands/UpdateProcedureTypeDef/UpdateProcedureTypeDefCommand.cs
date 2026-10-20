using MediatR;
using KLMN.Application.ProcedureTypes.Responses;
namespace KLMN.Application.ProcedureTypes.Commands.UpdateProcedureTypeDef;
/// <summary>Tetkik Türü Tanımları: Update komutu.</summary>
public sealed record UpdateProcedureTypeDefCommand(Guid Id,uint Version,string Name):IRequest<ProcedureTypeDefMutationResponse>;
