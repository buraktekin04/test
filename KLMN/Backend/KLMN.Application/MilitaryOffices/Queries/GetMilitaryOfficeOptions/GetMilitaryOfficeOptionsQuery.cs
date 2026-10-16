using MediatR;

namespace KLMN.Application.MilitaryOffices.Queries.GetMilitaryOfficeOptions;

/// <summary>Şube yetkisiyle şehir seçimlerini alır; ayrı Cities izni aranmaz.</summary>
public sealed record GetMilitaryOfficeOptionsQuery(Guid? SelectedCityId = null)
    : IRequest<IReadOnlyList<MilitaryOfficeOptionResponse>>;

/// <summary>Şehir seçimi için sınırlı kimlik, kod ve ad yanıtı.</summary>
public sealed record MilitaryOfficeOptionResponse(Guid Id, string Code, string Name);
