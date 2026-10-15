using KLMN.Application.Common.Models;
using KLMN.Application.MilitaryUnits.Responses;
using MediatR;

namespace KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitDefs;

/// <summary>Birlik tanımlarında arama, filtreleme ve sayfalama sorgusudur.</summary>
public sealed record GetMilitaryUnitDefsQuery(
    string? Search = null,
    bool IncludeInactive = false,
    int PageNumber = 1,
    int PageSize = 20,
    bool? IsActive = null,
    bool? RootOnly = null,
    Guid? ForcesCommandDef = null,
    Guid? ConnectedTopUnity = null)
    : IRequest<PagedResult<MilitaryUnitDefResponse>>;
