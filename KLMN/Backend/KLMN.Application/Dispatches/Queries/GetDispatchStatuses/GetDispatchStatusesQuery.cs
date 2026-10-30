using KLMN.Application.Dispatches.Responses;
using MediatR;

namespace KLMN.Application.Dispatches.Queries.GetDispatchStatuses;

/// <summary>Etkin sevk durumlarını veritabanından listeleme.</summary>
public sealed record GetDispatchStatusesQuery()
    : IRequest<IReadOnlyList<DispatchStatusOptionResponse>>;
