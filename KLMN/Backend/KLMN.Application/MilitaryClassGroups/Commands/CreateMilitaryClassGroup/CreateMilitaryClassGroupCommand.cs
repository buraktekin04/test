using MediatR;
using KLMN.Application.MilitaryClassGroups.Responses;

namespace KLMN.Application.MilitaryClassGroups.Commands.CreateMilitaryClassGroup;

/// <summary>Belirli kuvvet altında yeni askerî sınıf grubu oluşturur.</summary>
public sealed record CreateMilitaryClassGroupCommand(Guid ForcesCommandId, string Code, string Name, string? ShortName, DateTime? ValidFrom, DateTime? ValidTo)
    : IRequest<MilitaryClassGroupMutationResponse>;
