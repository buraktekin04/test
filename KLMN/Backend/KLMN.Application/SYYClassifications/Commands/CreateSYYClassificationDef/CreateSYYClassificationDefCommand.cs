using MediatR;
using KLMN.Application.SYYClassifications.Responses;
namespace KLMN.Application.SYYClassifications.Commands.CreateSYYClassificationDef;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi Create CQRS komutu.</summary>
public sealed record CreateSYYClassificationDefCommand(Guid SYYDiagnosisDefId,Guid ForcesCommandDef,Guid RankGroup,Guid MilitaryClassGroup,Guid HcDecision,DateTime? ValidFrom,DateTime? ValidTo):IRequest<SYYClassificationDefMutationResponse>;
