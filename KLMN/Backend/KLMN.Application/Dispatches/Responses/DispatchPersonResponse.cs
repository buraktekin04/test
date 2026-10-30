using KLMN.Application.Dispatches.Models;

namespace KLMN.Application.Dispatches.Responses;

/// <summary>Nüfus entegrasyonundan eşleştirilmiş kişi form bilgileri.</summary>
public sealed record DispatchPersonResponse(DispatchPersonInput Person);
