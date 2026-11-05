using KLMN.Application.Provisions.Responses;
using MediatR;

namespace KLMN.Application.Provisions.Commands.CancelProvision;

/// <summary>Provizyon iptal açıklaması zorunludur; xmin kontrolü uygulanır.</summary>
public sealed record CancelProvisionCommand(Guid Id,uint Version,string Reason)
    : IRequest<ProvisionMutationResponse>;
