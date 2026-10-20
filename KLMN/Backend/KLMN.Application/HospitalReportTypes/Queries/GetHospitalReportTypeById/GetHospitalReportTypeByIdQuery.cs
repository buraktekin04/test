using KLMN.Application.HospitalReportTypes.Responses;
using MediatR;

namespace KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypeById;

/// <summary>xmin içeren sağlık tesisi rapor türü detay sorgusu.</summary>
public sealed record GetHospitalReportTypeByIdQuery(Guid Id) : IRequest<HospitalReportTypeResponse>;
