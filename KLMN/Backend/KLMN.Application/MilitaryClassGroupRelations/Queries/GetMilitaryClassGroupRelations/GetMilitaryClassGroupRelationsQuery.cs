using KLMN.Application.Common.Models;
using KLMN.Application.MilitaryClassGroupRelations.Responses;
using MediatR;

namespace KLMN.Application.MilitaryClassGroupRelations.Queries.GetMilitaryClassGroupRelations;

/// <summary>Üçlü eşleştirmeyi seçili kuvvet/sınıf/grup, arama ve aktiflikle listeler.</summary>
public sealed record GetMilitaryClassGroupRelationsQuery(
    string? Search = null, bool IncludeInactive = false, int PageNumber = 1, int PageSize = 20,
    bool? IsActive = null, Guid? ForcesCommandId = null,
    Guid? MilitaryClassId = null, Guid? MilitaryClassGroupId = null)
    : IRequest<PagedResult<MilitaryClassGroupRelationResponse>>;
