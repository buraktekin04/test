using KLMN.Application.ReportTypes.Responses;
using MediatR;

namespace KLMN.Application.ReportTypes.Queries.GetReportTypeDefById;

/// <summary>xmin içeren rapor türü detay sorgusu.</summary>
public sealed record GetReportTypeDefByIdQuery(Guid Id) : IRequest<ReportTypeDefResponse>;
