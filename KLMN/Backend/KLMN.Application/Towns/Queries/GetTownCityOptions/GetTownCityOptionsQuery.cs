using MediatR;
namespace KLMN.Application.Towns.Queries.GetTownCityOptions;
/// <summary>İlçe tanımına ait sorgu, yanıt veya iş kuralı bileşeni.</summary>
public sealed record GetTownCityOptionsQuery(Guid? SelectedCityId = null)
    : IRequest<IReadOnlyList<TownCityOptionResponse>>;
/// <summary>İlçe formunda seçilebilir şehir bilgisi.</summary>
public sealed record TownCityOptionResponse(Guid Id, string Code, string Name);
