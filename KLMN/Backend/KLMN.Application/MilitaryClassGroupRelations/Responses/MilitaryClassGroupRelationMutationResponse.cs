using System.Linq.Expressions;
using KLMN.Domain.Entities.MilitaryUnit;

namespace KLMN.Application.MilitaryClassGroupRelations.Responses;

/// <summary>Eşleştirme sonrası Id ve yeni PostgreSQL xmin sürümü.</summary>
public sealed record MilitaryClassGroupRelationMutationResponse(Guid Id, uint Version);
