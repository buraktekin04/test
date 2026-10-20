using MediatR;

namespace KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypeOptions;

/// <summary>Tesis ve rapor türü seçenekleri.</summary>
public sealed record GetHospitalReportTypeOptionsQuery(Guid? SelectedHospitalDefId, Guid? SelectedReportTypeDefId)
    : IRequest<HospitalReportTypeOptionsResponse>;
public sealed record DefinitionOption(Guid Id, string Code, string Name);
public sealed record HospitalReportTypeOptionsResponse(
    IReadOnlyList<DefinitionOption> Hospitals, IReadOnlyList<DefinitionOption> ReportTypes);
