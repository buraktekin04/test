namespace KLMN.Application.Dispatches.Responses;

/// <summary>Dosyanın içerik taşımayan güvenli meta verisi.</summary>
public sealed record DispatchFileResponse(Guid Id,string FileName,string ContentType,int FileSize,DateTime CreatedDate);
