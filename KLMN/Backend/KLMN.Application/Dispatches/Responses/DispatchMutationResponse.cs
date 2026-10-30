namespace KLMN.Application.Dispatches.Responses;

/// <summary>Kaydedilen sevkin sıra numarası, kimliği ve xmin sürümüdür.</summary>
public sealed record DispatchMutationResponse(Guid Id, long DispatchNo, uint Version);
