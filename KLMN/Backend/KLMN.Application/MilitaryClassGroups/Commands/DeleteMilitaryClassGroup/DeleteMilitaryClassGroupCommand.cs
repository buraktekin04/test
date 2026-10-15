using MediatR;

namespace KLMN.Application.MilitaryClassGroups.Commands.DeleteMilitaryClassGroup;

/// <summary>Grubu ilişkileri koruyarak soft-delete yapar.</summary>
public sealed record DeleteMilitaryClassGroupCommand(Guid Id, uint Version)
    : IRequest;
