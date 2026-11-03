namespace KLMN.Application.Dispatches.Responses;

/// <summary>Mevcut tanım kayıtlarından gelen aramalı açılır liste seçeneği.</summary>
/// <summary>
/// Kind yalnız birleşik Gönderen Makam seçeneği için "unit"/"office" değerini alır.
/// Diğer tanımların önceki Id/Label/Code JSON sözleşmesi korunur.
/// </summary>
public sealed record DispatchOption(Guid Id,string Label,string? Code=null,string? Kind=null,bool? IsActive=null);
