using MediatR;
using KLMN.Application.ICD10Defs.Responses;
namespace KLMN.Application.ICD10Defs.Commands.UpdateICD10Def;
/// <summary>ICD10 Tanımları: Update CQRS komutu.</summary>
public sealed record UpdateICD10DefCommand(Guid Id,uint Version,string Code,string Name):IRequest<ICD10DefMutationResponse>;
