using MediatR;
using KLMN.Application.MilitaryClasses.Responses;

namespace KLMN.Application.MilitaryClasses.Commands.UpdateMilitaryClassDef;

/// <summary>Askerî sınıfı güncel xmin sürümüyle düzenleyen komut.</summary>
public sealed record UpdateMilitaryClassDefCommand(Guid Id, uint Version, string Code, string Name, string? ShortName, string? ExternalCode, DateTime? ValidFrom, DateTime? ValidTo)
    : IRequest<MilitaryClassDefMutationResponse>;
