using KLMN.Application.ReportTypes.Responses;
using MediatR;

namespace KLMN.Application.ReportTypes.Commands.UpdateReportTypeDef;

/// <summary>Rapor Türü Update CQRS komutu.</summary>
public sealed record UpdateReportTypeDefCommand(Guid Id, uint Version, string Code, string Name, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<ReportTypeDefMutationResponse>;
