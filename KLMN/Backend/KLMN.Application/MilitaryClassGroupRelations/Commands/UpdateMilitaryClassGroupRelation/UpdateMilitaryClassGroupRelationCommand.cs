using MediatR;
using KLMN.Application.MilitaryClassGroupRelations.Responses;

namespace KLMN.Application.MilitaryClassGroupRelations.Commands.UpdateMilitaryClassGroupRelation;

/// <summary>Üçlü eşleştirmeyi xmin sürümüyle günceller.</summary>
public sealed record UpdateMilitaryClassGroupRelationCommand(Guid Id, uint Version, Guid ForcesCommandId, Guid MilitaryClassId, Guid MilitaryClassGroupId)
    : IRequest<MilitaryClassGroupRelationMutationResponse>;
