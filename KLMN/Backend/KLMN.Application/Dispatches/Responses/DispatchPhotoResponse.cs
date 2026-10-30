namespace KLMN.Application.Dispatches.Responses;

/// <summary>İzin kontrolünden sonra gönderilen sevk kişisi fotoğrafı.</summary>
public sealed record DispatchPhotoResponse(string ContentType,byte[] Content);
