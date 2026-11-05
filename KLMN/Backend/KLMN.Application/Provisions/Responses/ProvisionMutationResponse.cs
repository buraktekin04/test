namespace KLMN.Application.Provisions.Responses;

/// <summary>Provizyon kaydı veya durumu değiştikten sonra dönen kimlik, numara ve xmin.</summary>
public sealed record ProvisionMutationResponse(Guid Id,long ProvisionNo,Guid DispatchId,uint Version);
