using KLMN.Application.Dispatches.Responses;
using MediatR;

namespace KLMN.Application.Dispatches.Queries.LookupDispatchPerson;

/// <summary>
/// Nüfus/MERNİS entegrasyonuna iletilecek kimlik sorgusu.
/// Yerel Persons tablosunu aramaz; gerçek kurum servisi handler içinde bağlanacaktır.
/// </summary>
public sealed record LookupDispatchPersonQuery(string? TcKimlikNo,string? YabanciKimlikNo,DateOnly BirthDate)
    : IRequest<DispatchPersonResponse?>;
