using MediatR;
using KLMN.Application.MilitaryClassGroups.Responses;
namespace KLMN.Application.MilitaryClassGroups.Commands.ChangeStatusMilitaryClassGroup;

/// <summary>Grubun IsActive durumunu güvenle değiştirir.</summary>
public sealed record ChangeStatusMilitaryClassGroupCommand(Guid Id, uint Version, bool IsActive)
    : IRequest<MilitaryClassGroupMutationResponse>;
