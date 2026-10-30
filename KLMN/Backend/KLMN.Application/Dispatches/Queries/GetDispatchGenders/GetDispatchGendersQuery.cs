using KLMN.Application.Dispatches.Responses;
using MediatR;

namespace KLMN.Application.Dispatches.Queries.GetDispatchGenders;

public sealed record GetDispatchGendersQuery()
    : IRequest<IReadOnlyList<DispatchGenderOptionResponse>>;
