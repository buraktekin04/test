using MediatR;
using KLMN.Application.MilitaryClassGroupRelations.Responses;
namespace KLMN.Application.MilitaryClassGroupRelations.Commands.ChangeStatusMilitaryClassGroupRelation;

/// <summary>İlişki aktifliğini yalnızca parent kayıtlar uygunsa değiştirir.</summary>
public sealed record ChangeStatusMilitaryClassGroupRelationCommand(Guid Id, uint Version, bool IsActive)
    : IRequest<MilitaryClassGroupRelationMutationResponse>;
