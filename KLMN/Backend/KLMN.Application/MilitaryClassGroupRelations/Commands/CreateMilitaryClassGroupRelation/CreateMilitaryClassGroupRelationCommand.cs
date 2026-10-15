using MediatR;
using KLMN.Application.MilitaryClassGroupRelations.Responses;

namespace KLMN.Application.MilitaryClassGroupRelations.Commands.CreateMilitaryClassGroupRelation;

/// <summary>Askerî sınıf ile grubu belirli kuvvet altında eşleştirir.</summary>
public sealed record CreateMilitaryClassGroupRelationCommand(Guid ForcesCommandId, Guid MilitaryClassId, Guid MilitaryClassGroupId)
    : IRequest<MilitaryClassGroupRelationMutationResponse>;
