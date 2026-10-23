using System.Linq.Expressions;
using KLMN.Domain.Entities.MilitaryUnit;

namespace KLMN.Application.MilitaryClassGroups.Responses;

/// <summary>Değişiklik sonrası kimlik ve PostgreSQL xmin sürümüdür.</summary>
public sealed record MilitaryClassGroupMutationResponse(Guid Id, uint Version);
