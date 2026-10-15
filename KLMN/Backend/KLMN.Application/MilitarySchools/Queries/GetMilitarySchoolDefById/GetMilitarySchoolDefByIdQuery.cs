using KLMN.Application.MilitarySchools.Responses;
using MediatR;

namespace KLMN.Application.MilitarySchools.Queries.GetMilitarySchoolDefById;

/// <summary>Düzenleme öncesi askerî okul kaydının güncel xmin'ini getirir.</summary>
public sealed record GetMilitarySchoolDefByIdQuery(Guid Id) : IRequest<MilitarySchoolDefResponse>;
