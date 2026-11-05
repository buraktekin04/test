using KLMN.Application.Provisions.Responses;
using MediatR;

namespace KLMN.Application.Provisions.Commands.CreateWithDispatch;

/// <summary>Eski Provizyon Sevkli: seçili sevk GUID + kabul eden hastane GUID.</summary>
public sealed record CreateWithDispatchCommand(Guid? DispatchId,Guid HospitalId,long? DispatchNo=null)
    : IRequest<ProvisionMutationResponse>;
