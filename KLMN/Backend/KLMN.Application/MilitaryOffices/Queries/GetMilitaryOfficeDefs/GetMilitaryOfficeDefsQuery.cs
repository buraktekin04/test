using KLMN.Application.Common.Models;
using KLMN.Application.MilitaryOffices.Responses;
using MediatR;

namespace KLMN.Application.MilitaryOffices.Queries.GetMilitaryOfficeDefs;

/// <summary>Askerlik şubelerini arama, şehir, durum ve sayfalama ile sorgular.</summary>
public sealed record GetMilitaryOfficeDefsQuery(
    string? Search = null, bool IncludeInactive = false,
    int PageNumber = 1, int PageSize = 20,
    bool? IsActive = null, Guid? CityId = null)
    : IRequest<PagedResult<MilitaryOfficeDefResponse>>;
