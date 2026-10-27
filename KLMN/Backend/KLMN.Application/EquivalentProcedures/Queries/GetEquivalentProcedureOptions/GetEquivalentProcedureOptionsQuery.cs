using KLMN.Application.EquivalentProcedures.Responses;
using MediatR;
namespace KLMN.Application.EquivalentProcedures.Queries.GetEquivalentProcedureOptions;
/// <summary>İsteğe bağlı arama; p-autocomplete dropdown için en fazla 60 kayıt.</summary>
public sealed record GetEquivalentProcedureOptionsQuery(
 string? Search=null,Guid? SelectedId=null):IRequest<IReadOnlyList<EquivalentProcedureOption>>;
