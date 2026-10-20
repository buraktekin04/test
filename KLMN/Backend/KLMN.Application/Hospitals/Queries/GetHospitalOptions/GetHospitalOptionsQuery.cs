using MediatR;

namespace KLMN.Application.Hospitals.Queries.GetHospitalOptions;

/// <summary>Sağlık Tesisi ekranı şehir ve hakem tesisi dropdown sorgusu.</summary>
public sealed record GetHospitalOptionsQuery(Guid? SelectedCityId, Guid? SelectedRefereeHospitalId)
    : IRequest<HospitalOptionsResponse>;
public sealed record HospitalOption(Guid Id, string Code, string Name);
public sealed record HospitalOptionsResponse(IReadOnlyList<HospitalOption> Cities,
    IReadOnlyList<HospitalOption> RefereeHospitals);
