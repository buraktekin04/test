using KLMN.Application.Dispatches.Models;
using KLMN.Application.Provisions.Responses;
using MediatR;

namespace KLMN.Application.Provisions.Commands.CreateWithoutDispatch;

/// <summary>Sevksiz provizyon: kişi ve askerî/başvuru verileriyle tek işlem.</summary>
public sealed record CreateWithoutDispatchCommand(
    Guid HospitalId,DispatchPersonInput Person,DispatchDetailsInput Dispatch,
    Guid? ObjectionableReportId=null,string? PhotoBase64=null) : IRequest<ProvisionMutationResponse>;
