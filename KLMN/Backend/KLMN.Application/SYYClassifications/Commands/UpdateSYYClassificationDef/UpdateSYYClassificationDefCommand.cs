using MediatR;
using KLMN.Application.SYYClassifications.Responses;
namespace KLMN.Application.SYYClassifications.Commands.UpdateSYYClassificationDef;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi Update CQRS komutu.</summary>
public sealed record UpdateSYYClassificationDefCommand(Guid Id,uint Version,Guid SYYDiagnosisDefId,Guid ForcesCommandDef,Guid RankGroup,Guid MilitaryClassGroup,Guid HcDecision,DateTime? ValidFrom,DateTime? ValidTo):IRequest<SYYClassificationDefMutationResponse>;
