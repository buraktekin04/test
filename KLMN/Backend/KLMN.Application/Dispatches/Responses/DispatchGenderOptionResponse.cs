namespace KLMN.Application.Dispatches.Responses;

/// <summary>Cinsiyet seçimlerinin DB'den gelen kod/ad değerleri.</summary>
public sealed record DispatchGenderOptionResponse(int Code,string Name);
