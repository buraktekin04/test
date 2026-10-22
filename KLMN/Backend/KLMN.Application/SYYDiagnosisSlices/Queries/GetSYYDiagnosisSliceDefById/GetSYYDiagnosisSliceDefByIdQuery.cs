using KLMN.Application.SYYDiagnosisSlices.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnosisSlices.Queries.GetSYYDiagnosisSliceDefById;
/// <summary>SYY Dilim Tanımları tek kayıt sorgusu.</summary>
public sealed record GetSYYDiagnosisSliceDefByIdQuery(Guid Id):IRequest<SYYDiagnosisSliceDefResponse>;
