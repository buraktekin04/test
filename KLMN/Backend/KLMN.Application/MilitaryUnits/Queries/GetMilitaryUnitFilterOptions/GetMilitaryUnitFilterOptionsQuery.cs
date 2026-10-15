using MediatR;

namespace KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitFilterOptions;

/// <summary>Birlik listesinde kullanılacak silinmemiş kuvvet ve üst birlik seçenekleri.</summary>
public sealed record GetMilitaryUnitFilterOptionsQuery() : IRequest<MilitaryUnitFilterOptionsResponse>;

/// <summary>Dropdown'da gösterilen kod, ad ve varsa bağlı kuvvet adıdır.</summary>
public sealed record MilitaryUnitFilterOption(Guid Id, string Code, string Name, string? ForceName);

/// <summary>Formdaki aktif kayıt seçeneklerinden bağımsız liste filtre verisidir.</summary>
public sealed record MilitaryUnitFilterOptionsResponse(
    IReadOnlyList<MilitaryUnitFilterOption> Forces,
    IReadOnlyList<MilitaryUnitFilterOption> Parents);
