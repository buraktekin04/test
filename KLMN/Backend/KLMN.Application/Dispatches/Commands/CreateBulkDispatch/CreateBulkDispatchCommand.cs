using KLMN.Application.Dispatches.Models;
using KLMN.Application.Dispatches.Responses;
using MediatR;

namespace KLMN.Application.Dispatches.Commands.CreateBulkDispatch;

/// <summary>En fazla 100 sevki ayrı transaction'larla oluşturan toplu komut.</summary>
public sealed record CreateBulkDispatchCommand(IReadOnlyList<BulkDispatchItemInput> Items)
    : IRequest<BulkDispatchResponse>;
