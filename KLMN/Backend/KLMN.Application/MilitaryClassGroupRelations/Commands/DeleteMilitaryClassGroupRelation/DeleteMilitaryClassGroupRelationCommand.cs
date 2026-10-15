using MediatR;

namespace KLMN.Application.MilitaryClassGroupRelations.Commands.DeleteMilitaryClassGroupRelation;

/// <summary>Eşleştirmeyi soft-delete yapar.</summary>
public sealed record DeleteMilitaryClassGroupRelationCommand(Guid Id, uint Version)
    : IRequest;
