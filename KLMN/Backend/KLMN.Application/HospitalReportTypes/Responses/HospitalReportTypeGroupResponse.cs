namespace KLMN.Application.HospitalReportTypes.Responses;

/// <summary>Bir sağlık tesisi ve tek tabloda alt alta gösterilecek rapor eşleştirmeleri.</summary>
public sealed record HospitalReportTypeGroupResponse(
    Guid HospitalDefId,
    string HospitalCode,
    string HospitalName,
    IReadOnlyList<HospitalReportTypeResponse> ReportTypes);
