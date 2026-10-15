using MediatR;
using KLMN.Application.MilitaryClasses.Responses;

namespace KLMN.Application.MilitaryClasses.Commands.CreateMilitaryClassDef;

/// <summary>Kod, ad, kısa ad, harici kod, geçerlilik alanlarıyla yeni sınıf oluşturur.</summary>
public sealed record CreateMilitaryClassDefCommand(string Code, string Name, string? ShortName, string? ExternalCode, DateTime? ValidFrom, DateTime? ValidTo)
    : IRequest<MilitaryClassDefMutationResponse>;
