using MediatR;
using KLMN.Application.ICD10Defs.Responses;
namespace KLMN.Application.ICD10Defs.Commands.CreateICD10Def;
/// <summary>ICD10 Tanımları: Create CQRS komutu.</summary>
public sealed record CreateICD10DefCommand(string Code,string Name):IRequest<ICD10DefMutationResponse>;
