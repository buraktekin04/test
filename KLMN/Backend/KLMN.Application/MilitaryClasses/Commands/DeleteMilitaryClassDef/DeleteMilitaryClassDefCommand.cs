using MediatR;


namespace KLMN.Application.MilitaryClasses.Commands.DeleteMilitaryClassDef;

/// <summary>Askerî sınıfın soft-delete komutu; xmin koruması kullanır.</summary>
public sealed record DeleteMilitaryClassDefCommand(Guid Id, uint Version)
    : IRequest;
