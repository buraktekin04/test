using KLMN.Application.Common.Models;
using KLMN.Application.ForcesCommands.Responses;
using MediatR;

namespace KLMN.Application.ForcesCommands.Queries.GetForcesCommandDefs;

/// <summary>Kuvvet komutanlığı tanımlarında arama, filtreleme ve sayfalama sorgusudur.</summary>
public sealed record GetForcesCommandDefsQuery(
    string? Search = null,
    bool IncludeInactive = false,
    int PageNumber = 1,
    int PageSize = 20,
    bool? IsActive = null,
    string SortField = "name",
    int SortOrder = 1)
    : IRequest<PagedResult<ForcesCommandDefResponse>>;
