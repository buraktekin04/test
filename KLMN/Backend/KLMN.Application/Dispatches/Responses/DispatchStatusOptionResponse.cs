namespace KLMN.Application.Dispatches.Responses;

/// <summary>Sevk durum seçimlerini veritabanından döndüren küçük referans modeli.</summary>
public sealed record DispatchStatusOptionResponse(int Code,string Name);
