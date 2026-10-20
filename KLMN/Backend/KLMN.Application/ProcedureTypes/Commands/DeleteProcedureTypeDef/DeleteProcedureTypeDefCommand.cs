using MediatR;

namespace KLMN.Application.ProcedureTypes.Commands.DeleteProcedureTypeDef;
/// <summary>Tetkik Türü Tanımları: Delete komutu.</summary>
public sealed record DeleteProcedureTypeDefCommand(Guid Id,uint Version):IRequest;
