using KLMN.Application.Common.Models;
using KLMN.Application.MilitarySchools.Responses;
using MediatR;

namespace KLMN.Application.MilitarySchools.Queries.GetMilitarySchoolDefs;

/// <summary>Askerî okulları şehir/kuvvet, durum, arama ve sayfaya göre sorgular.</summary>
public sealed record GetMilitarySchoolDefsQuery(
    string? Search = null, bool IncludeInactive = false, int PageNumber = 1,
    int PageSize = 20, bool? IsActive = null,
    Guid? CityId = null, Guid? ForcesCommandDefId = null)
    : IRequest<PagedResult<MilitarySchoolDefResponse>>;
