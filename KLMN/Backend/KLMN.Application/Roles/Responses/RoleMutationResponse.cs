

namespace KLMN.Application.Roles.Responses;

/// <summary>Rol oluşturma/düzenleme/aktiflik/yetki işlemlerinin güncel versiyon cevabıdır.</summary>
public sealed record RoleMutationResponse(Guid Id, uint Version);
