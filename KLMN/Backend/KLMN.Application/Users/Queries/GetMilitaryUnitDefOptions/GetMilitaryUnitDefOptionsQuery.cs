using KLMN.Application.Users.Responses;
using MediatR;

namespace KLMN.Application.Users.Queries.GetMilitaryUnitDefOptions;

/// <summary>
/// Kullanıcı görev yeri seçiminde kullanılmak üzere aktif birlik
/// birimlerini döndürür; rol veya yetki bilgilerini içermez.
/// </summary>
public sealed record GetMilitaryUnitDefOptionsQuery
    : IRequest<IReadOnlyList<MilitaryUnitDefOptionResponse>>;
