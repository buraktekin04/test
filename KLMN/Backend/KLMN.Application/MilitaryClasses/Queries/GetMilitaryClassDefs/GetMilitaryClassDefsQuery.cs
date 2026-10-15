using KLMN.Application.Common.Models;
using KLMN.Application.MilitaryClasses.Responses;
using MediatR;

namespace KLMN.Application.MilitaryClasses.Queries.GetMilitaryClassDefs;

/// <summary>Arama, sistem aktifliği, eski nullable Active ve sayfalama parametrelerini taşır.</summary>
public sealed record GetMilitaryClassDefsQuery(
    string? Search = null, bool IncludeInactive = false, int PageNumber = 1,
    int PageSize = 20, bool? IsActive = null, bool? Active = null)
    : IRequest<PagedResult<MilitaryClassDefResponse>>;
