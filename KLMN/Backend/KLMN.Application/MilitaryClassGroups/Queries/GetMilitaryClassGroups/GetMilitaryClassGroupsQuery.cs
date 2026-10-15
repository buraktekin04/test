using KLMN.Application.Common.Models;
using KLMN.Application.MilitaryClassGroups.Responses;
using MediatR;

namespace KLMN.Application.MilitaryClassGroups.Queries.GetMilitaryClassGroups;

/// <summary>Grupları arama, kuvvet ve kayıt aktifliğiyle sayfalı getirir.</summary>
public sealed record GetMilitaryClassGroupsQuery(
    string? Search = null, bool IncludeInactive = false, int PageNumber = 1,
    int PageSize = 20, bool? IsActive = null, Guid? ForcesCommandId = null)
    : IRequest<PagedResult<MilitaryClassGroupResponse>>;
