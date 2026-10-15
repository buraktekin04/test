using MediatR;

namespace KLMN.Application.MilitarySchools.Queries.GetMilitarySchoolOptions;

/// <summary>
/// Askerî okulun şehir ve kuvvet dropdown'ları için kendi permission'ıyla
/// erişilen kayıtları ister; iki referans modülüne ayrıca yetki gerekmez.
/// </summary>
public sealed record GetMilitarySchoolOptionsQuery(Guid? SelectedCityId = null, Guid? SelectedForcesCommandDefId = null)
    : IRequest<MilitarySchoolOptionsResponse>;

/// <summary>Dropdown'da yalnızca kimlik, kod ve ad gibi güvenli referans verileri bulunur.</summary>
public sealed record MilitarySchoolOptionResponse(Guid Id, string Code, string Name);

/// <summary>Şehir ve kuvvet komutanlığı seçimlerini bir HTTP yanıtında sunar.</summary>
public sealed record MilitarySchoolOptionsResponse(
    IReadOnlyList<MilitarySchoolOptionResponse> Cities,
    IReadOnlyList<MilitarySchoolOptionResponse> ForcesCommands);
