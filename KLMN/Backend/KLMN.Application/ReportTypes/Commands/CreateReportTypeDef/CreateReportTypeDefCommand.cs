using KLMN.Application.ReportTypes.Responses;
using MediatR;

namespace KLMN.Application.ReportTypes.Commands.CreateReportTypeDef;

/// <summary>Rapor Türü Create CQRS komutu.</summary>
public sealed record CreateReportTypeDefCommand(string Code, string Name, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<ReportTypeDefMutationResponse>;
