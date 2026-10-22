using KLMN.Application.SYYDiagnosisTypes.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnosisTypes.Queries.GetSYYDiagnosisTypeDefById;
/// <summary>SYY Tanı Türü Tanımları tek kayıt sorgusu.</summary>
public sealed record GetSYYDiagnosisTypeDefByIdQuery(Guid Id):IRequest<SYYDiagnosisTypeDefResponse>;
