using KLMN.Application.SYYDiagnosisBranches.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnosisBranches.Queries.GetSYYDiagnosisBranchDefById;
public sealed record GetSYYDiagnosisBranchDefByIdQuery(Guid Id):IRequest<SYYDiagnosisBranchDefResponse>;
