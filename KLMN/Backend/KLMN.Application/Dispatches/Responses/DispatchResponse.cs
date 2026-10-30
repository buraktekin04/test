using KLMN.Application.Dispatches.Models;
namespace KLMN.Application.Dispatches.Responses;

/// <summary>Görüntüleme ve düzenleme ekranında kullanılan tam sevk detayı.</summary>
public sealed record DispatchResponse(Guid Id,long DispatchNo,DateTime DispatchDate,int CurrentState,
 bool IsActive,uint Version,bool HasPhoto,DispatchPersonInput Person,DispatchDetailsInput Dispatch);
