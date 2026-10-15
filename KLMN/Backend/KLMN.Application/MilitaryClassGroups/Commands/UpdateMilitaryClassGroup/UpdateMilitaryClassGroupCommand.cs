using MediatR;
using KLMN.Application.MilitaryClassGroups.Responses;

namespace KLMN.Application.MilitaryClassGroups.Commands.UpdateMilitaryClassGroup;

/// <summary>Güncel xmin ile grup ve kuvvet bağlantısını düzenler.</summary>
public sealed record UpdateMilitaryClassGroupCommand(Guid Id, uint Version, Guid ForcesCommandId, string Code, string Name, string? ShortName, DateTime? ValidFrom, DateTime? ValidTo)
    : IRequest<MilitaryClassGroupMutationResponse>;
