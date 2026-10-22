using KLMN.Application.SYYDiagnoses.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnoses.Queries.GetSYYDiagnosisDefById;
/// <summary>SYY Tanı Listesi (M/D/F) tek kayıt sorgusu.</summary>
public sealed record GetSYYDiagnosisDefByIdQuery(Guid Id):IRequest<SYYDiagnosisDefResponse>;
