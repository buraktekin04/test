using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ICD10Defs.Responses;
public sealed record ICD10DefMutationResponse(Guid Id,uint Version);
