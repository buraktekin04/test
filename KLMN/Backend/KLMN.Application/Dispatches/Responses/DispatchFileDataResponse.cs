namespace KLMN.Application.Dispatches.Responses;
/// <summary>İndirme izni sağlandıktan sonra istek başına okunan dosya içeriği.</summary>
public sealed record DispatchFileDataResponse(string FileName,string ContentType,byte[] Content);
