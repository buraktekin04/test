using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ICD10Diagnostics.Responses;
public sealed record ICD10DiagnosticMutationResponse(Guid Id,uint Version);
