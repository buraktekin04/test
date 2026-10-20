using MediatR;
using KLMN.Application.ProcedureTypes.Responses;
namespace KLMN.Application.ProcedureTypes.Commands.CreateProcedureTypeDef;
/// <summary>Tetkik Türü Tanımları: Create komutu.</summary>
public sealed record CreateProcedureTypeDefCommand(int Code,string Name):IRequest<ProcedureTypeDefMutationResponse>;
