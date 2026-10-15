using MediatR;
using KLMN.Application.MilitaryClasses.Responses;

namespace KLMN.Application.MilitaryClasses.Queries.GetMilitaryClassDefById;

/// <summary>İşlem öncesinde güncel askerî sınıf detayını ve xmin sürümünü ister.</summary>
public sealed record GetMilitaryClassDefByIdQuery(Guid Id) : IRequest<MilitaryClassDefResponse>;
