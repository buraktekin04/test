using KLMN.Application.Dispatches.Responses;
using MediatR;
namespace KLMN.Application.Dispatches.Queries.GetDispatchAdmissionRules;
/// <summary>Sevk formunun dinamik zorunluluk ve muayene sebebi kullanılmama kuralları.</summary>
public sealed record GetDispatchAdmissionRulesQuery(Guid AdmissionTypeId):IRequest<DispatchAdmissionRulesResponse>;
