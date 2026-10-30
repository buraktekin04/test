using KLMN.Application.Dispatches.Models;

namespace KLMN.Application.Dispatches.Responses;

/// <summary>Nüfus entegrasyonundan kişi ve opsiyonel askerî tanım bilgileri.</summary>
public sealed record DispatchPersonResponse(
    DispatchPersonInput Person,DispatchRegistryMilitaryResponse? Military=null);
