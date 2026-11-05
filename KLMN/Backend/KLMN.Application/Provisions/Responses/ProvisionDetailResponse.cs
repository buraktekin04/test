using KLMN.Application.Dispatches.Models;

namespace KLMN.Application.Provisions.Responses;

/// <summary>Provizyon ve ilişkili sevk/kişi detayları.</summary>
public sealed record ProvisionDetailResponse(
    Guid Id,long ProvisionNo,Guid DispatchId,long DispatchNo,DateTime ProvisionDate,
    Guid HospitalId,int CurrentState,int? WithoutDispatch,Guid? ObjectionableReportId,
    string? Description,uint Version,DispatchPersonInput Person,DispatchDetailsInput Dispatch,
    IReadOnlyList<ProvisionStateResponse> States);
