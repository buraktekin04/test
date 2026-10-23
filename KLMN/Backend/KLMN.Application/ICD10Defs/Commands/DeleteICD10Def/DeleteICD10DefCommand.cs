using MediatR;

namespace KLMN.Application.ICD10Defs.Commands.DeleteICD10Def;
/// <summary>ICD10 Tanımları: Delete CQRS komutu.</summary>
public sealed record DeleteICD10DefCommand(Guid Id,uint Version):IRequest;
